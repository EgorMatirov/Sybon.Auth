using System.Threading.Tasks;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth.Services.UsersService
{
    public interface IUsersService
    {
        Task<User> FindAsync(long id);
        Task<long> AddAsync(User user);
        Task RemoveAsync(long id);
        Task<User> FindByLoginAsync(string login);
        Task SetTokenIdAsync(long userId, long tokenId);
        Task<User.RoleType> GetUserRoleAsync(long userId);
    }
}