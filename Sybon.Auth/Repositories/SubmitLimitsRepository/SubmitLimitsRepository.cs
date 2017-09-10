using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;

namespace Sybon.Auth.Repositories.SubmitLimitsRepository
{
    [UsedImplicitly]
    public class SubmitLimitsRepository : BaseEntityRepository<SubmitLimit, long>, ISubmitLimitsRepository
    {
        public SubmitLimitsRepository(AuthContext context) : base(context)
        {
        }

        public Task<SubmitLimit> FindByUserIdAsync(long userId)
        {
            return Context.SubmitLimits.SingleOrDefaultAsync(x => x.UserId == userId);
        }
        
        public Task<SubmitLimit> GetByUserIdAsync(long userId)
        {
            return Context.SubmitLimits.SingleAsync(x => x.UserId == userId);
        }
    }
}