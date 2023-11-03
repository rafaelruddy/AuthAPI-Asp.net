using AuthApi.Repository;
using AuthApi.Repository.Interfaces;
using AuthApi.Data;
using AuthApi.Models.Dtos;
using AuthApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthApi.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly AuthContext _context;
        public UserRepository(AuthContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {

            return await _context.Users
                    .Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(x => new UserDto { Id = x.Id, UserName = x.UserName })
                    .ToListAsync();
        }
    }
}
