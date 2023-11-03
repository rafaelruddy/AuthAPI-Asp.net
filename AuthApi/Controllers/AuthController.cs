
using AuthApi.Helpers;
using AuthApi.Models.Dtos;
using AuthApi.Models.Entities;
using AuthApi.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Services;

namespace AuthApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        public IConfiguration _configuration { get; }


        public AuthController(IUserRepository userRepository, IMapper mapper, UserService userService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            var user = HttpContext.User;

            var claim = user.FindFirst("id");

            if (claim != null)
            {

                var valorDaClaim = Int32.Parse(claim.Value);
                await _userRepository.GetUserByIdAsync(valorDaClaim);

                return Ok(valorDaClaim);
            }

            return BadRequest("a");
        }


        [HttpPost("Login")]
        public IActionResult Login(LoginDto model)
        {
            var user = _userService.Authenticate(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok("Usuario autenticado com sucesso.");

        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto newUser)
        {
            // map model to entity
            var user = _mapper.Map<User>(newUser);

            try
            {
                // create user
                _userService.Create(user, newUser.Password);
                return Ok("Usuario criado com sucesso.");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
