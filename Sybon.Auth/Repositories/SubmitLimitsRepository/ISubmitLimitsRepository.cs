using System.Threading.Tasks;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;

namespace Sybon.Auth.Repositories.SubmitLimitsRepository
{
    public interface ISubmitLimitsRepository : IBaseEntityRepository<SubmitLimit, long>
    {
        Task<SubmitLimit> FindByUserIdAsync(long userId);
        Task<SubmitLimit> GetByUserIdAsync(long userId);
    }
}