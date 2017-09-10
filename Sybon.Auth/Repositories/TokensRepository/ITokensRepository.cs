using System.Threading.Tasks;
using Sybon.Auth.Repositories.TokensRepository.Entities;

namespace Sybon.Auth.Repositories.TokensRepository
{
    public interface ITokensRepository : IBaseEntityRepository<Token, long>
    {
        Task<Token> FindByKeyAsync(string key);
    }
}