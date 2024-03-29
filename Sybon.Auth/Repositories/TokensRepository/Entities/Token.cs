﻿using System;
using System.ComponentModel.DataAnnotations;
using Sybon.Auth.Repositories.UsersRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.TokensRepository.Entities
{
    public class Token : IEntity
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Key { get; set; }
        public DateTime? ExpireTime { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}