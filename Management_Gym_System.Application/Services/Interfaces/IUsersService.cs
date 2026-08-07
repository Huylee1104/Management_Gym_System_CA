using Management_Gym_System.Domain.Entities;

public interface IUsersService
{
    Task<List<UserDto>> GetUsers(string? keyword, long? filterValue);
    Task<User> CreateUser(UserCreateUpdateDto request);
    Task<bool> UpdateUser(long id,UserCreateUpdateDto request);
    Task<User> ToggleStatus(long id);
    Task<bool> Delete(int id);
}