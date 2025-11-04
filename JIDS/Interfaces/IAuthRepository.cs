using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetInteriorApp.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> ValidateUserAsync(string username, string plainPassword);
        Task<bool> RegisterUserAsync(string username, string email, string plainPassword);
    }

}
