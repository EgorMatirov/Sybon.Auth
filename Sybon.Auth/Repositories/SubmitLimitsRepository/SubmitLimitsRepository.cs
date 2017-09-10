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
        public SubmitLimitsRepository(EntityContext<SubmitLimit> context) : base(context)
        {
        }

        public Task<SubmitLimit> FindByUserIdAsync(long userId)
        {
            return Context.Entities.SingleOrDefaultAsync(x => x.UserId == userId);
        }
        
        public Task<SubmitLimit> GetByUserIdAsync(long userId)
        {
            return Context.Entities.SingleAsync(x => x.UserId == userId);
        }
    }
}