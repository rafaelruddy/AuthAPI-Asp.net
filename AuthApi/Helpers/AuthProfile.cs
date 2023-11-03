using AuthApi.Models.Dtos;
using AuthApi.Models.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AuthApi.Helpers
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, RegisterDto>();
            CreateMap<RegisterDto, User>();
            CreateMap<User, LoginDto>();
            CreateMap<LoginDto, User>();
            CreateMap<User, UserDto>();

        }
    }
}
