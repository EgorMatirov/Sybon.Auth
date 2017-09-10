using System.Threading.Tasks;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;

namespace Sybon.Auth.Repositories.CollectionPermissionsRepository
{
    public interface ICollectionPermissionsRepository : IBaseEntityRepository<CollectionPermission, long>
    {
        Task<CollectionPermission> FindByUserAndCollectionAsync(long userId, long collectionId);
    }
}