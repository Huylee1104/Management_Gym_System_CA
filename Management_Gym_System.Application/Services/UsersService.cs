using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepo;

    public UsersService(IUsersRepository usersRepo)
    {
        _usersRepo = usersRepo;
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
        var user = new User
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            RoleID = request.RoleID,
            Avatar = request.Avatar,
            Status = request.Status ?? true
        };

        await _usersRepo.AddAsync(user);

        if (request.GoiTapID.HasValue)
        {
            // Tìm thẻ chưa gán user nhưng đã có RFID
            var membership = await _usersRepo.GetGymMembershipCardByIdAsync();

            // Không còn thẻ trống
            if (membership == null)
            {
                return new User();
            }

            var startDate = DateTime.UtcNow;

            // Map user vào thẻ
            membership.UserID = user.ID;
            membership.ProductID = request.GoiTapID;

            membership.StartDate = startDate;

            membership.EndDate = request.ThoiHan.HasValue
                ? startDate.AddDays(request.ThoiHan.Value)
                : null;

            await _usersRepo.SaveChangesAsync();
        }

        return user;
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
        return true;
    }

    public async Task<User> ToggleStatus(long id)
    {
        var existingUser = await _usersRepo.GetUserByIdAsync(id);
        if (existingUser == null)
            return new User();

        existingUser.Status = !existingUser.Status;
        await _usersRepo.UpdateAsync(existingUser);
        return existingUser;
    }

    public async Task<bool> Delete(int id)
    {
        var existingUser = await _usersRepo.GetUserByIdAsync(id);
        if (existingUser == null)
            return false;

        await _usersRepo.DeleteAsync(existingUser);
        return true;
    }
}