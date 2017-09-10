using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;

namespace Sybon.Auth.Repositories.CollectionPermissionsRepository
{
    [UsedImplicitly]
    public class CollectionPermissionsRepository : BaseEntityRepository<CollectionPermission, long>, ICollectionPermissionsRepository
    {
        public CollectionPermissionsRepository(EntityContext<CollectionPermission> context) : base(context)
        {
        }

        public Task<CollectionPermission> FindByUserAndCollectionAsync(long userId, long collectionId)
        {
            return Context.Entities.SingleOrDefaultAsync(x => x.UserId == userId && x.CollectionId == collectionId);
        }
    }
}