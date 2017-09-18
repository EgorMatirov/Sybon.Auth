using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities
{
    public class CollectionPermission : IEntity
    {
        public enum PermissionType
        {
            None,
            Read,
            ReadAndWrite
        }
        
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long CollectionId { get; set; }
        public PermissionType Type { get; set; }
    }
}