using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.TokensRepository.Entities;

namespace Sybon.Auth.Repositories.TokensRepository
{
    [UsedImplicitly]
    public class TokensRepository : BaseEntityRepository<Token, long>, ITokensRepository
    {
        public TokensRepository(EntityContext<Token> context) : base(context)
        {
        }

        public Task<Token> FindByKeyAsync(string key)
        {
            return Context.Entities.SingleOrDefaultAsync(token => string.Equals(token.Key, key));
        }
    }
}