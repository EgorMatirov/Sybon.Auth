using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Auth.Repositories.TokensRepository;
using Sybon.Auth.Services.AccountService.Models;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Auth.Services.UsersService;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.Services.AccountService
{
    [UsedImplicitly]
    public class AccountService : IAccountService
    {
        private readonly IUsersService _usersService;
        private readonly IPasswordsService _passwordsService;
        private readonly ITokensRepository _tokensRepository;
        private readonly ITokensConverter _tokensConverter;

        public AccountService(
            IUsersService usersService,
            IPasswordsService passwordsService,
            ITokensRepository tokensRepository,
            ITokensConverter tokensConverter)
        {
            _usersService = usersService;
            _passwordsService = passwordsService;
            _tokensRepository = tokensRepository;
            _tokensConverter = tokensConverter;
        }

        public async Task<Token> AuthAsync(string login, string password)
        {
            var user = await _usersService.FindByLoginAsync(login);
            if (user == null)
                return null;
            if (!PasswordMatches(user, password))
                return null;
            
            var token = new Repositories.TokensRepository.Entities.Token
            {
                Key = GenerateToken(),
                ExpireTime = DateTime.Now.Add(TimeSpan.FromDays(1)),
                UserId = user.Id
            };

            await _tokensRepository.AddAsync(token);
            await _tokensRepository.SaveAsync();
            await _usersService.SetTokenIdAsync(user.Id, token.Id);

            return _tokensConverter.Convert(token);
        }

        private static string GenerateToken()
        {
            var guid = Guid.NewGuid();
            var token = Convert.ToBase64String(guid.ToByteArray())
                .Where(char.IsLetterOrDigit);
            return string.Join(string.Empty, token);
        }

        private bool PasswordMatches(User user, string password)
        {
            return user.Password == _passwordsService.HashPassword(password);
        }

        public async Task<Token> CheckTokenAsync(string key)
        {
            var token = await _tokensRepository.FindByKeyAsync(key);
            if (token == null) return null;
            var user = await _usersService.FindAsync(token.UserId);
            return user != null ? _tokensConverter.Convert(token) : null;
        }
    }
}