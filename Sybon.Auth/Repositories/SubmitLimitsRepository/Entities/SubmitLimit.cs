using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sybon.Auth.Repositories.SubmitLimitsRepository.Entities
{
    public class SubmitLimit : IEntity<long>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int MinuteLimit { get; set; }
        public int MonthLimit { get; set; }
        [DefaultValue(0)]
        public int SubmitsDuringMonth { get; set; }
        [Required]
        public DateTime SubmitsRefreshDate { get; set; }
    }
}