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

    public UsersService(IUsersRepository usersRepo, IUnitOfWork unitOfWork, IPermissionService permission, IMembershipAuditLogRepository auditlog)
    {
        _usersRepo = usersRepo;
        _unitOfWork = unitOfWork;
        _permission = permission;
        _auditlog = auditlog;
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
            var audit = new MembershipAuditLog
            {
                Date = DateTime.UtcNow,
                StaffId = idStaff,
                Action = "Register",
                MemberId = user.ID,
                Note = "Đăng ký mới cho hội viên: " + user.FullName,
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
            Note = "Đăng ký mới cho hội viên: " + existingUser.FullName,
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