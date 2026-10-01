using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs.Identities;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _siginManager;
        private readonly ApplicationDbContext _context;
        public AuthController(UserManager<User> userManager, SignInManager<User> siginManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _siginManager = siginManager;
            _context = context;
        }
        [HttpPost("Register")] //Post : api/Auth/Register
        public async Task<ActionResult<UserDto> Registration(RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //check email
            var Email = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (Email != null) return BadRequest(new { Message = "This Email is already registered" });

            //Check  userName
            var userName = registerRequestDto.UserName;
            var existingUserName = await _userManager.FindByNameAsync(userName);
            if (existingUserName != null)
                return BadRequest(new { message = "This username is already taken." });

            //create new User
            var user = new User()
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                Role = registerRequestDto.role

            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) return Unauthorized(401);

            var result = await _siginManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return Unauthorized(401);

            return Ok(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = "Token"


            });
        }
    }
}
