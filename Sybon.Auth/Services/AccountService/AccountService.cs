using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Auth.Repositories.TokensRepository;
using Sybon.Auth.Repositories.UsersRepository;
using Sybon.Auth.Services.AccountService.Models;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Common;

namespace Sybon.Auth.Services.AccountService
{
    [UsedImplicitly]
    public class AccountService : IAccountService
    {
        private readonly IPasswordsService _passwordsService;
        private readonly IMapper _mapper;
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;

        public AccountService(
            IPasswordsService passwordsService,
            IMapper mapper,
            IRepositoryUnitOfWork repositoryUnitOfWork)
        {
            _passwordsService = passwordsService;
            _mapper = mapper;
            _repositoryUnitOfWork = repositoryUnitOfWork;
        }

        public async Task<Token> AuthAsync(string login, string password)
        {
            var user = await  _repositoryUnitOfWork.GetRepository<IUsersRepository>().FindByLoginAsync(login);
            if (user == null)
                return null;
            if (!PasswordMatches(user, password))
                return null;

            var token = new Repositories.TokensRepository.Entities.Token
            {
                UserId = user.Id,
                Key = GenerateToken(),
                ExpireTime = DateTime.UtcNow.Add(TimeSpan.FromDays(1))
            };

            user.Tokens = user.Tokens.Append(token).ToArray();

            await _repositoryUnitOfWork.SaveChangesAsync();
            return _mapper.Map<Token>(token);
        }

        private static string GenerateToken()
        {
            var guid = Guid.NewGuid();
            var token = Convert.ToBase64String(guid.ToByteArray())
                .Where(char.IsLetterOrDigit);
            return string.Join(string.Empty, token);
        }

        private bool PasswordMatches(Repositories.UsersRepository.Entities.User user, string password)
        {
            return user.Password == _passwordsService.HashPassword(password);
        }

        public async Task<Token> CheckTokenAsync(string key)
        {
            var token = await _repositoryUnitOfWork.GetRepository<ITokensRepository>().FindByKeyAsync(key);
            return token?.User != null ? _mapper.Map<Token>(token) : null;
        }
    }
}