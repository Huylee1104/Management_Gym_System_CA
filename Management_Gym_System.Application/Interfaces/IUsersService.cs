using Management_Gym_System.Domain.Entities;

public interface IUsersService
{
    Task<List<UserDto>> GetUsers(string? keyword, long? filterValue);
    Task<List<UserDto>> GetStaffs(string? keyword, long? filterValue);
    Task<ServiceResult> CreateUser(UserCreateUpdateDto request);
    Task<ServiceResult> UpdateUser(long id,UserCreateUpdateDto request);
    Task<User> ToggleStatus(long id);
    Task<bool> Delete(long id);
}