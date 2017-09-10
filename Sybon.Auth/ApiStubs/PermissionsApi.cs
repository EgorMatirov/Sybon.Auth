using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Auth.Services.UsersService;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.ApiStubs
{
    [UsedImplicitly]
    public class PermissionsApi : IPermissionsApi
    {
        private readonly IUsersService _usersService;

        public PermissionsApi(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public Task<User.RoleType> GetUserRoleAsync(long userId)
        {
            return _usersService.GetUserRoleAsync(userId);
        }
    }
}