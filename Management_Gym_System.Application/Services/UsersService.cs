using System.Security.Claims;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permission;
    private readonly IMembershipAuditLogRepository _auditlog;
    private readonly IRolesRepository _roleRepo;
    private readonly IProductRepository _productRepo;

    public UsersService(IUsersRepository usersRepo, IUnitOfWork unitOfWork, IPermissionService permission, IMembershipAuditLogRepository auditlog, IRolesRepository roleRepo, IProductRepository productRepo)
    {
        _usersRepo = usersRepo;
        _unitOfWork = unitOfWork;
        _permission = permission;
        _auditlog = auditlog;
        _roleRepo = roleRepo;
        _productRepo = productRepo;
    }

    public async Task<List<UserDto>> GetUsers(string? keyword, long? filterValue)
    {
        var users = await _usersRepo.GetAllUsersAsync(keyword, filterValue);
        return users.Select(u => new UserDto
        {
            Id = u.ID,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Status = u.Status,
            RoleID = u.RoleID,
            RoleName = u.Role?.RoleName,
            Avatar = u.Avatar,
            GoiTapID = u.Memberships.FirstOrDefault()?.ProductID,
            GoiTapName = u.Memberships.FirstOrDefault()?.Product?.ProductName
        }).ToList();
    }

    public async Task<User> CreateUser(UserCreateUpdateDto request)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                RoleID = request.RoleID,
                Avatar = request.Avatar,
                Status = request.Status ?? true
            };

            await _usersRepo.AddAsync(user);

            // Lưu User trước để lấy user.ID
            await _usersRepo.SaveChangesAsync();

            if (request.GoiTapID.HasValue)
            {
                // Tìm thẻ chưa gán user nhưng đã có RFID
                var membership = await _usersRepo.GetGymMembershipCardByIdAsync();

                // Không còn thẻ trống
                if (membership == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new User();
                }

                var startDate = DateTime.UtcNow;

                membership.UserID = user.ID;
                membership.ProductID = request.GoiTapID;
                membership.StartDate = startDate;
                membership.EndDate = request.ThoiHan.HasValue
                    ? startDate.AddDays(request.ThoiHan.Value)
                    : null;

                await _usersRepo.UpdateAsync(membership);

                // Lưu MembershipCard
                await _usersRepo.SaveChangesAsync();
            }

            var idStaff = _permission.GetUserId();

            var changes = new List<object>();

            if (request.FullName != null)
            {
                changes.Add(new
                {
                    Field = "FullName",
                    Display = "Họ và tên",
                    Old = "",
                    New = request.FullName
                });
            }

            if (request.PhoneNumber != null)
            {
                changes.Add(new
                {
                    Field = "PhoneNumber",
                    Display = "Số điện thoại",
                    Old = "",
                    New = request.PhoneNumber
                });
            }

            if (request.RoleID != null)
            {
                var newRole = request.RoleID.HasValue
                    ? await _roleRepo.GetRoleByIdAsync(request.RoleID.Value)
                    : null;

                changes.Add(new
                {
                    Field = "RoleID",
                    Display = "Vai trò",
                    Old = "",
                    New = newRole?.RoleName ?? "Không có vai trò"
                });
            }

            if (request.Status != null)
            {
                changes.Add(new
                {
                    Field = "Status",
                    Display = "Trạng thái",
                    Old = "",
                    New = request.Status == true ? "Hoạt động" : "Ngưng hoạt động"
                });
            }

            if (request.GoiTapID != null)
            {
                var goiTap = await _productRepo.GetByIdAsync(request.GoiTapID.Value);
                changes.Add(new
                {
                    Field = "GoiTapID",
                    Display = "Gói tập",
                    Old = "",
                    New = goiTap?.ProductName ?? "Không có gói tập"
                });
            }

            var audit = new MembershipAuditLog
            {
                Date = DateTime.UtcNow,
                StaffId = idStaff,
                Action = "Register",
                MemberId = user.ID,
                Note = "Đăng ký mới cho hội viên: " + user.FullName,
                DataEdited = System.Text.Json.JsonSerializer.Serialize(changes)
            };

            await _auditlog.AddAsync(audit);

            await _unitOfWork.CommitAsync();

            return user;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateUser(long id, UserCreateUpdateDto request)
    {
        var existingUser = await _usersRepo.GetUserByIdAsync(id);
        if (existingUser == null)
            return false;

        var changes = new List<object>();

        if (existingUser.FullName != request.FullName)
        {
            changes.Add(new
            {
                Field = "FullName",
                Display = "Họ và tên",
                Old = existingUser.FullName,
                New = request.FullName
            });
        }

        if (existingUser.PhoneNumber != request.PhoneNumber)
        {
            changes.Add(new
            {
                Field = "PhoneNumber",
                Display = "Số điện thoại",
                Old = existingUser.PhoneNumber,
                New = request.PhoneNumber
            });
        }

        if (existingUser.RoleID != request.RoleID)
        {
            var newRole = request.RoleID.HasValue
                ? await _roleRepo.GetRoleByIdAsync(request.RoleID.Value)
                : null;

            changes.Add(new
            {
                Field = "RoleID",
                Display = "Vai trò",
                Old = existingUser.Role?.RoleName ?? "Không có vai trò",
                New = newRole?.RoleName ?? "Không có vai trò"
            });
        }

        if (existingUser.Status != request.Status)
        {
            changes.Add(new
            {
                Field = "Status",
                Display = "Trạng thái",
                Old = existingUser.Status == true ? "Hoạt động" : "Ngưng hoạt động",
                New = request.Status == true ? "Hoạt động" : "Ngưng hoạt động"
            });
        }

        existingUser.FullName = request.FullName;
        existingUser.PhoneNumber = request.PhoneNumber;
        existingUser.RoleID = request.RoleID;
        existingUser.Avatar = request.Avatar;
        existingUser.Status = request.Status ?? existingUser.Status;

        await _usersRepo.UpdateAsync(existingUser);

        var idStaff = _permission.GetUserId();

        var audit = new MembershipAuditLog
        {
            Date = DateTime.UtcNow,
            StaffId = idStaff,
            Action = "EditUser",
            MemberId = id,
            Note = "Chỉnh sửa thông tin hội viên: " + existingUser.FullName,
            DataEdited = System.Text.Json.JsonSerializer.Serialize(changes)
        };

        await _auditlog.AddAsync(audit);

        await _usersRepo.SaveChangesAsync();
        return true;
    }

    public async Task<User> ToggleStatus(long id)
    {
        var existingUser = await _usersRepo.GetUserByIdAsync(id);
        if (existingUser == null)
            return new User();

        existingUser.Status = !existingUser.Status;
        await _usersRepo.UpdateAsync(existingUser);
        await _usersRepo.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> Delete(long id)
    {
        var existingUser = await _usersRepo.GetUserByIdAsync(id);
        var udCard = await _usersRepo.UpdateGymMembershipCard(id);
        if (udCard == false)
        {
            return false;
        }
        await _usersRepo.SaveChangesAsync();
        if (existingUser == null)
            return false;

        await _usersRepo.DeleteAsync(existingUser);
        await _usersRepo.SaveChangesAsync();
        return true;
    }
}