using System.Threading.Tasks;
using Sybon.Auth.Services.AccountService.Models;

namespace Sybon.Auth.Services.AccountService
{
    public interface IAccountService
    {
        Task<Token> AuthAsync(string login, string password);
        Task<Token> CheckTokenAsync(string key);
    }
}