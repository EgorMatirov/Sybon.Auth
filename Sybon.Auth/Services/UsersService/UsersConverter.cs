using JetBrains.Annotations;
using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth.Services.UsersService
{
    [UsedImplicitly]
    public class UsersConverter : IUsersConverter
    {
        public Models.User Convert(User user)
        {
            if (user == null) return null;
            return new Models.User
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                Password = user.Password,
                TokenId = user.TokenId,
                Role =  (Models.User.RoleType) user.Role
            };
        }

        public User Convert(Models.User user)
        {
            if (user == null) return null;
            return new User
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                Password = user.Password,
                TokenId = user.TokenId,
                Role =  (User.RoleType) user.Role
            };
        }
    }
}