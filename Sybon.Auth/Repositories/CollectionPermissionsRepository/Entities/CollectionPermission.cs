namespace Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities
{
    public class CollectionPermission : IEntity<long>
    {
        public enum PermissionType
        {
            None,
            Read,
            ReadAndWrite
        }
        
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CollectionId { get; set; }
        public PermissionType Type { get; set; }
    }
}