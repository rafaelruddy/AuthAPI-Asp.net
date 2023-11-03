using AuthApi.Repository.Interfaces;
using AuthApi.Data;
using AuthApi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AuthApi.Helpers;
using AuthApi.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        
        void Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private AuthContext _context;
        private readonly IUserRepository _userRepository;
        public IConfiguration _configuration { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AuthContext context, IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.UserName == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            Console.WriteLine(user.PasswordHash);


            var match = CheckPassword(password, user);
            if (!match)
            {
                return null;
            }

            JWTGenerator(user);

            // authentication successful
            return user;
        }


        public void Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

         

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            _context.Users.Add(user);
            _context.SaveChanges();

        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.UserName) && userParam.UserName != user.UserName)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");

                user.UserName = userParam.UserName;
            }

            // update user properties if provided
            //if (!string.IsNullOrWhiteSpace(userParam.FirstName))
            //    user.FirstName = userParam.FirstName;

            //if (!string.IsNullOrWhiteSpace(userParam.LastName))
            //    user.LastName = userParam.LastName;

            // update password if provided
            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    byte[] passwordHash, passwordSalt;
            //    using (HMACSHA512? hmac = new HMACSHA512())
            //    {
            //        user.PasswordSalt = hmac.Key;
            //        user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordHash));
            //    }
            //}

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private bool CheckPassword(string password, User user)
        {
            bool result;

            using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
            {
                var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                result = compute.SequenceEqual(user.PasswordHash);
            }

            return result;
        }

        public dynamic JWTGenerator(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["ApplicationSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.DateOfBirth, user.BirthDay)}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            SetJWT(encrypterToken);

            var refreshToken = GenerateRefreshToken();

            SetRefreshToken(refreshToken, user);

            return new { token = encrypterToken, username = user.UserName };
        }

        private RefreshTokenDto GenerateRefreshToken()
        {
            var refreshToken = new RefreshTokenDto()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;

        }

        public async void SetRefreshToken(RefreshTokenDto refreshToken, User user)
        {

            _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
                 new CookieOptions
                 {
                     Expires = refreshToken.Expires,
                     HttpOnly = true,
                     Secure = true,
                     IsEssential = true,
                     SameSite = SameSiteMode.None
                 });

            var refreshUser = await _userRepository.GetUserByUsernameAsync(user.UserName);

            refreshUser.Token = refreshToken.Token;
            refreshUser.TokenCreated = refreshToken.Created;
            refreshUser.TokenExpires = refreshToken.Expires;
        }

        public void SetJWT(string encrypterToken)
        {

            _httpContextAccessor.HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddMinutes(15),
                      HttpOnly = true,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
        }

        Task<User> IUserService.Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}