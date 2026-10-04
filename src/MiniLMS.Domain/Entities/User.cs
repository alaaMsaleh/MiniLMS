using Microsoft.AspNetCore.Identity;

namespace MiniLMS.Domain.Entities
{
    public class User : IdentityUser<int>
    {


        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }
}
