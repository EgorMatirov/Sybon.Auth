using System.Threading.Tasks;
using Sybon.Auth.Services.AccountService.Models;

namespace Sybon.Auth.ApiStubs
{
    public interface IAccountApi
    {
        // ReSharper disable once InconsistentNaming
        Task<Token> CheckTokenAsync(string api_key);
    }
}