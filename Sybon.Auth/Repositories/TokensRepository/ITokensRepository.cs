using System.Threading.Tasks;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.TokensRepository
{
    public interface ITokensRepository : IBaseEntityRepository<Token>
    {
        Task<Token> FindByKeyAsync(string key);
    }
}