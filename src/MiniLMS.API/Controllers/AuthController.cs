using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs.Identities;
using MiniLMS.Domain.Entities;
using MiniLMS.Domain.ServicesContract;

namespace MiniLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _siginManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        public AuthController(UserManager<User> userManager, SignInManager<User> siginManager,
            ILogger<AuthController> logger,
            IAuthService authService)
        {
            _userManager = userManager;
            _siginManager = siginManager;
            _authService = authService;
            _logger = logger;

        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) return Unauthorized(401);

            var result = user is null
          ? null
          : await _siginManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (user is null || result is null || !result.Succeeded)
            {
                _logger.LogWarning("Failed login attempt for {Email}", model.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            _logger.LogInformation("User {UserId} logged in", user.Id);

            return Ok(new UserDto()
            {
                UserName = user.FullName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)


            });
        }
    }
}
