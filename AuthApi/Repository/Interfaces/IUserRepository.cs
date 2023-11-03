
using AuthApi.Models.Dtos;
using AuthApi.Models.Entities;
using AuthApi.Repository.Interfaces;

namespace AuthApi.Repository.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();

        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
