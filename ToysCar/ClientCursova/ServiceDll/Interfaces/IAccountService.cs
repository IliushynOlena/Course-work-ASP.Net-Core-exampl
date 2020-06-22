using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    interface IAccountService
    {
        Dictionary<string, string> Login(AccountModel model);
        Task<Dictionary<string, string>> LoginAsync(AccountModel model);

        bool Logout(string userEmail);
        Task<bool> LogoutAsync(string userEmail);

        Dictionary<string, string> Registration(UserModel model);
        Task<Dictionary<string, string>> RegistrationAsync(UserModel model);
    }
}
