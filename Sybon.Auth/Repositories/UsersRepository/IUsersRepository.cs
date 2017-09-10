using System.Threading.Tasks;
using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth.Repositories.UsersRepository
{
    public interface IUsersRepository : IBaseEntityRepository<User, long>
    {
        Task<User> FindByLoginAsync(string login);
    }
}