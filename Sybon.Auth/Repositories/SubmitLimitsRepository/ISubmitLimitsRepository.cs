﻿using System.Threading.Tasks;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;
using Sybon.Common;

namespace Sybon.Auth.Repositories.SubmitLimitsRepository
{
    public interface ISubmitLimitsRepository : IBaseEntityRepository<SubmitLimit>
    {
        Task<SubmitLimit> FindByUserIdAsync(long userId);
        Task<SubmitLimit> GetByUserIdAsync(long userId);
    }
}