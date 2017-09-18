using System;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Auth.Repositories.UsersRepository;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.Services.UsersService
{
    [UsedImplicitly]
    public class UsersService : IUsersService
    {
        private readonly IMapper _mapper;
        private readonly IPasswordsService _passwordsService;
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;

        public UsersService(IMapper mapper, IPasswordsService passwordsService, IRepositoryUnitOfWork repositoryUnitOfWork)
        {
            _mapper = mapper;
            _passwordsService = passwordsService;
            _repositoryUnitOfWork = repositoryUnitOfWork;
        }

        public async Task<User> FindAsync(long id)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<IUsersRepository>().FindAsync(id);
            return _mapper.Map<User>(dbEntry);
        }

        public async Task<long> AddAsync(User user)
        {
            user.Password = _passwordsService.HashPassword(user.Password);
            var dbEntry = _mapper.Map<Repositories.UsersRepository.Entities.User>(user);
            var result = await  _repositoryUnitOfWork.GetRepository<IUsersRepository>().AddAsync(dbEntry);
            await _repositoryUnitOfWork.SaveChangesAsync();
            return result;
        }

        public Task RemoveAsync(long id)
        {
            _repositoryUnitOfWork.GetRepository<IUsersRepository>().RemoveAsync(id);
            return _repositoryUnitOfWork.SaveChangesAsync();
        }

        public async Task<User> FindByLoginAsync(string login)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<IUsersRepository>().FindByLoginAsync(login);
            return  _mapper.Map<User>(dbEntry);
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