using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniLMS.Domain.Entities;
using MiniLMS.Domain.ServicesContract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniLMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(User user, UserManager<User> _userManager)
        {
            //Playload (Claims )
            //Private Claims (User-defined

            var privateClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier ,user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email , user.Email!),


            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {

                privateClaims.Add(new Claim(ClaimTypes.Role, role));


            }


            //Secy Key

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:AuthKey"]));

            //object need to built token

            var token = new JwtSecurityToken(

               issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"] ?? "60")),
                claims: privateClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)

                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
