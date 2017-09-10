using System.Threading.Tasks;
using Sybon.Auth.Services.PermissionsService.Models;

namespace Sybon.Auth.Services.PermissionsService
{
    public interface IPermissionsService
    {
        Task<PermissionType> GetToProblemAsync(long userId, long problemId);
        Task<PermissionType> GetToCollectionAsync(long userId, long collectionId);
        Task AddToCollectionAsync(long userId, long collectionId, PermissionType permission);
        bool TryIncreaseSubmitsCount(long userId, int cnt);
    }
}