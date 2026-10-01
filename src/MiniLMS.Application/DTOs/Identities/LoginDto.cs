using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.Identities
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
