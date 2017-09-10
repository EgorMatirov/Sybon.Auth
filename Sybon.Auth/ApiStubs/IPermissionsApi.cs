using System.Threading.Tasks;
using Sybon.Auth.Services.PermissionsService;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.ApiStubs
{
    public interface IPermissionsApi
    {
        Task<User.RoleType> GetUserRoleAsync(long userId);
    }
}