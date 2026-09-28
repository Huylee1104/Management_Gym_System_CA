using System.Globalization;
using System.Security.Claims;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permission;
    private readonly IMembershipAuditLogRepository _auditlog;
    private readonly IRolesRepository _roleRepo;
    private readonly IProductRepository _productRepo;
    private readonly IMemoryCache _cache;


    public UsersService(IUsersRepository usersRepo, IUnitOfWork unitOfWork, IPermissionService permission, IMembershipAuditLogRepository auditlog,
        IRolesRepository roleRepo, IProductRepository productRepo, IMemoryCache cache)
    {
        _usersRepo = usersRepo;
        _unitOfWork = unitOfWork;
        _permission = permission;
        _auditlog = auditlog;
        _roleRepo = roleRepo;
        _productRepo = productRepo;
        _cache = cache;
    }

    private const string STAFF_CACHE_KEY = "STAFF_LIST";

    public async Task<List<UserDto>> GetUsers(string? keyword, long? filterValue)
    {
        var users = await _usersRepo.GetAllUsersAsync();
        var staffs = users.Select(u => new UserDto
        {
            Id = u.ID,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Status = u.Status,
            RoleID = u.RoleID,
            RoleName = u.Role?.RoleName,
            Avatar = u.Avatar,
            GoiTapID = u.Memberships.FirstOrDefault()?.ProductID,
            GoiTapName = u.Memberships.FirstOrDefault()?.Product?.ProductName,
            NgaySinh = u.NgaySinh,
            GioiTinh = u.GioiTinh,
            UserType = u.UserType
        }).ToList();

        if (filterValue.HasValue && filterValue.Value > 0 && staffs != null)
        {
            staffs = staffs.Where(x =>
                x.GoiTapID == filterValue.Value
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(keyword) && staffs != null)
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

            staffs = staffs.Where(x =>
                !string.IsNullOrWhiteSpace(x.FullName) &&
                compareInfo.IndexOf(
                    x.FullName,
                    keyword,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            ).ToList();
        }

        return staffs ?? new List<UserDto>();
    }

    public async Task<List<UserDto>> GetStaffs(string? keyword, long? filterValue)
    {
        if (!_cache.TryGetValue(STAFF_CACHE_KEY, out List<UserDto>? staffs))
        {
            var users = await _usersRepo.GetAllUsersAsync();
            staffs = users.Where(u => u.UserType != 1).Select(u => new UserDto
            {
                Id = u.ID,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                RoleID = u.RoleID,
                RoleName = u.Role?.RoleName,
                Avatar = u.Avatar,
                GoiTapID = u.Memberships.FirstOrDefault()?.ProductID,
                GoiTapName = u.Memberships.FirstOrDefault()?.Product?.ProductName,
                NgaySinh = u.NgaySinh,
                GioiTinh = u.GioiTinh,
                UserType = u.UserType
            }).ToList();

            _cache.Set(STAFF_CACHE_KEY, staffs);

        }

        if (filterValue.HasValue && filterValue.Value > 0 && staffs != null)
        {
            staffs = staffs.Where(x =>
                x.GoiTapID == filterValue.Value
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(keyword) && staffs != null)
        {
            var compareInfo = CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

            staffs = staffs.Where(x =>
                !string.IsNullOrWhiteSpace(x.FullName) &&
                compareInfo.IndexOf(
                    x.FullName,
                    keyword,
                    CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                ) >= 0
            ).ToList();
        }

        return staffs ?? new List<UserDto>();
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
                Status = request.Status ?? true,
                NgaySinh = request.NgaySinh,
                GioiTinh = request.GioiTinh,
                UserType = request.UserType
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

            if (request.NgaySinh != null)
            {
                changes.Add(new
                {
                    Field = "NgaySinh",
                    Display = "Ngày sinh",
                    Old = "",
                    New = request.NgaySinh
                });
            }

            if (request.GioiTinh != null)
            {
                changes.Add(new
                {
                    Field = "GioiTinh",
                    Display = "Giới tính",
                    Old = "",
                    New = request.GioiTinh
                });
            }

            if (request.UserType != null)
            {
                changes.Add(new
                {
                    Field = "UserType",
                    Display = "Loại người dùng",
                    Old = "",
                    New = request.UserType == 1 ? "Hội viên" : "Nhân viên"
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

            if (request.UserType != 1)
            {
                _cache.Remove(STAFF_CACHE_KEY);
            }

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

        if (existingUser.NgaySinh != request.NgaySinh)
        {
            changes.Add(new
            {
                Field = "NgaySinh",
                Display = "Ngày sinh",
                Old = existingUser.NgaySinh,
                New = request.NgaySinh
            });
        }

        if (existingUser.GioiTinh != request.GioiTinh)
        {
            changes.Add(new
            {
                Field = "GioiTinh",
                Display = "Giới tính",
                Old = existingUser.GioiTinh,
                New = request.GioiTinh
            });
        }

        if (existingUser.UserType != request.UserType)
        {
            changes.Add(new
            {
                Field = "UserType",
                Display = "Loại người dùng",
                Old = existingUser.UserType == 1 ? "Hội viên" : "Nhân viên",
                New = request.UserType == 1 ? "Hội viên" : "Nhân viên"
            });
        }

        existingUser.FullName = request.FullName;
        existingUser.PhoneNumber = request.PhoneNumber;
        existingUser.RoleID = request.RoleID;
        existingUser.Avatar = request.Avatar;
        existingUser.Status = request.Status ?? existingUser.Status;
        existingUser.NgaySinh = request.NgaySinh;
        existingUser.GioiTinh = request.GioiTinh;
        existingUser.UserType = request.UserType;

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

        if (request.UserType != 1)
        {
            _cache.Remove(STAFF_CACHE_KEY);
        }

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

        if (existingUser.UserType != 1)
        {
            _cache.Remove(STAFF_CACHE_KEY);
        }

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

        if (existingUser.UserType != 1)
        {
            _cache.Remove(STAFF_CACHE_KEY);
        }

        return true;
    }
}