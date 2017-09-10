using System;
using System.ComponentModel.DataAnnotations;

namespace Sybon.Auth.Repositories.TokensRepository.Entities
{
    public class Token : IEntity<long>
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Key { get; set; }
        public DateTime? ExpireTime { get; set; }
        public long UserId { get; set; }
    }
}