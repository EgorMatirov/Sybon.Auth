using System.Threading.Tasks;
using Sybon.Auth.Repositories.UsersRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.UsersRepository
{
    public interface IUsersRepository : IBaseEntityRepository<User>
    {
        Task<User> FindByLoginAsync(string login);
    }
}