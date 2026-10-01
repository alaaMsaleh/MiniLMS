using Microsoft.AspNetCore.Identity;
using MiniLMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLMS.Domain.ServicesContract
{
    public interface IAuthService
    {
        //seignature to method implementation token

        Task<String> CreateTokenAsync(User user , UserManager<User> userManager) ;
    }
}
