using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.TokensRepository.Entities;

namespace Sybon.Auth.Repositories.TokensRepository
{
    [UsedImplicitly]
    public class TokensRepository : BaseEntityRepository<Token>, ITokensRepository
    {
        public TokensRepository(AuthContext context) : base(context)
        {
        }

        public Task<Token> FindByKeyAsync(string key)
        {
            return Context.Tokens
                .Include(token => token.User)
                .SingleOrDefaultAsync(token => string.Equals(token.Key, key));
        }
    }
}