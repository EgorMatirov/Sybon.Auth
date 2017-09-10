using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Auth.Repositories.UsersRepository;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.Services.UsersService
{
    [UsedImplicitly]
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersConverter _usersConverter;
        private readonly IPasswordsService _passwordsService;

        public UsersService(IUsersRepository usersRepository, IUsersConverter usersConverter, IPasswordsService passwordsService)
        {
            _usersRepository = usersRepository;
            _usersConverter = usersConverter;
            _passwordsService = passwordsService;
        }

        public async Task<User> FindAsync(long id)
        {
            var dbEntry = await _usersRepository.FindAsync(id);
            return _usersConverter.Convert(dbEntry);
        }

        public async Task<long> AddAsync(User user)
        {
            user.Password = _passwordsService.HashPassword(user.Password);
            var dbEntry = _usersConverter.Convert(user);
            var result = await _usersRepository.AddAsync(dbEntry);
            await _usersRepository.SaveAsync();
            return result;
        }

        public Task RemoveAsync(long id)
        {
            _usersRepository.RemoveAsync(id);
            return _usersRepository.SaveAsync();
        }

        public async Task<User> FindByLoginAsync(string login)
        {
            var dbEntry = await _usersRepository.FindByLoginAsync(login);
            return _usersConverter.Convert(dbEntry);
        }

        public async Task SetTokenIdAsync(long userId, long tokenId)
        {
            var dbEntry = await _usersRepository.FindAsync(userId);
            dbEntry.TokenId = tokenId;
            await _usersRepository.SaveAsync();
        }

        public async Task<User.RoleType> GetUserRoleAsync(long userId)
        {
            var user = await FindAsync(userId);
            if(user == null)
                throw new ArgumentException($"User with userId = {userId} is not found");
            return user.Role;
        }
    }
}