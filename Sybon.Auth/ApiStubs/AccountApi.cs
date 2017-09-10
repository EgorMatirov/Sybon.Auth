using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Auth.Services.AccountService;
using Sybon.Auth.Services.AccountService.Models;

namespace Sybon.Auth.ApiStubs
{
    [UsedImplicitly]
    public class AccountApi : IAccountApi
    {
        private readonly IAccountService _accountService;

        public AccountApi(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // ReSharper disable once InconsistentNaming
        public Task<Token> CheckTokenAsync(string api_key)
        {
            return _accountService.CheckTokenAsync(api_key);
        }
    }
}