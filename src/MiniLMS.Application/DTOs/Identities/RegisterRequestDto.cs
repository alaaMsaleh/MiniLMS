using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.Identities
{
    public class RegisterRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string role { get; set; }

    }
}
