using System.ComponentModel.DataAnnotations;

namespace Sybon.Auth.Repositories.UsersRepository.Entities
{
    public class User : IEntity<long>
    {
        public enum RoleType
        {
            User = 0,
            Admin = 1
        }
        
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Login { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        public long? TokenId { get; set; }
        public RoleType Role { get; set; }
        //public ICollection<CollectionPermission> ProblemPermissions { get; set; }
        //public SubmitLimit SubmitLimit { get; set; }
    }
}