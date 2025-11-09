using JIDS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIDS.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserDB?> ValidateUserAsync(string username, string plainPassword);
        Task<bool> RegisterUserAsync(string username, string email, string plainPassword);
    }

}
