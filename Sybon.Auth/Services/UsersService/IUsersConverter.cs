using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth.Services.UsersService
{
    public interface IUsersConverter
    {
        Models.User Convert(User user);
        User Convert(Models.User user);
    }
}