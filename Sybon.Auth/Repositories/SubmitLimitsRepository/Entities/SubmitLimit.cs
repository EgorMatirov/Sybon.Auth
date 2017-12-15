using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sybon.Auth.Repositories.UsersRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.SubmitLimitsRepository.Entities
{
    public class SubmitLimit : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public int MinuteLimit { get; set; }
        public int MonthLimit { get; set; }
        [DefaultValue(0)]
        public int SubmitsDuringMonth { get; set; }
        [Required]
        public DateTime SubmitsRefreshDate { get; set; }
    }
}