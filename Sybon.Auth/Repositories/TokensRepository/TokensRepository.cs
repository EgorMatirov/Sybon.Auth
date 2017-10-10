using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.TokensRepository
{
    [UsedImplicitly]
    public class TokensRepository : BaseEntityRepository<Token, AuthContext>, ITokensRepository
    {
        public TokensRepository(AuthContext context) : base(context)
        {
        }

        public Task<Token> FindByKeyAsync(string key)
        {
            return Context.Tokens
                .Include(token => token.User)
                .SingleOrDefaultAsync(token => token.Key == key);
        }
    }
}