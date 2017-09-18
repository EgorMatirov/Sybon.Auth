﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth.Repositories.UsersRepository
{
    [UsedImplicitly]
    public class UsersRepository : BaseEntityRepository<User>, IUsersRepository
    {
        public UsersRepository(AuthContext context) : base(context)
        {
        }

        public Task<User> FindByLoginAsync(string login)
        {
            return Context.Users.FirstOrDefaultAsync(x => x.Login == login);
        }
    }
}