using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.CollectionPermissionsRepository
{
    [UsedImplicitly]
    public class CollectionPermissionsRepository : BaseEntityRepository<CollectionPermission, AuthContext>, ICollectionPermissionsRepository
    {
        public CollectionPermissionsRepository(AuthContext context) : base(context)
        {
        }

        public Task<CollectionPermission> FindByUserAndCollectionAsync(long userId, long collectionId)
        {
            return Context.CollectionPermissions.SingleOrDefaultAsync(x => x.UserId == userId && x.CollectionId == collectionId);
        }
    }
}