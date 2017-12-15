using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.UsersRepository.Entities
{
    public class User : IEntity
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
        public Token Token { get; set; }
        public RoleType Role { get; set; }
        public ICollection<CollectionPermission> ProblemPermissions { get; set; }
        public SubmitLimit SubmitLimit { get; set; }
    }
}