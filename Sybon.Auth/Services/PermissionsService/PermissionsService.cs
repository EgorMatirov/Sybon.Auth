using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Archive.Client.Api;
using Sybon.Auth.ApiStubs;
using Sybon.Auth.Repositories.CollectionPermissionsRepository;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;
using Sybon.Auth.Repositories.SubmitLimitsRepository;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;
using Sybon.Auth.Services.PermissionsService.Models;
using Sybon.Auth.Services.UsersService;
using Sybon.Auth.Services.UsersService.Models;
using Sybon.Common;

namespace Sybon.Auth.Services.PermissionsService
{
    [UsedImplicitly]
    public class PermissionsService : IPermissionsService
    {
        private readonly IProblemsApi _problemsApi;
        private readonly IUsersService _usersService;
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private static readonly ConcurrentDictionary<long, LimitInfo> LimitInfos = 
            new ConcurrentDictionary<long, LimitInfo>();

        public PermissionsService(
            IProblemsApi problemsApi,
            IUsersService usersService,
            IRepositoryUnitOfWork repositoryUnitOfWork)
        {
            _problemsApi = problemsApi;
            _usersService = usersService;
            _repositoryUnitOfWork = repositoryUnitOfWork;
        }

        public async Task<PermissionType> GetToProblemAsync(long userId, long problemId)
        {
            if(await IsAdmin(userId))
                return PermissionType.ReadAndWrite;
            
            var problem = _problemsApi.GetById(problemId);
            return problem?.CollectionId == null
                ? PermissionType.None
                : await GetToCollectionAsync(userId, problem.CollectionId.Value);
        }

        private async Task<bool> IsAdmin(long userId)
        {
            var user = await _usersService.FindAsync(userId);
            return user?.Role == User.RoleType.Admin;
        }
        
        public async Task<PermissionType> GetToCollectionAsync(long userId, long collectionId)
        {
            if(await IsAdmin(userId))
                return PermissionType.ReadAndWrite;
            var permission = await _repositoryUnitOfWork.GetRepository<ICollectionPermissionsRepository>().FindByUserAndCollectionAsync(userId, collectionId);
            if (permission == null)
                return PermissionType.None;

            return (PermissionType) permission.Type;
        }
        
        public async Task AddToCollectionAsync(long userId, long collectionId, PermissionType permission)
        {
            var dbPermission = await _repositoryUnitOfWork.GetRepository<ICollectionPermissionsRepository>().FindByUserAndCollectionAsync(userId, collectionId);
            if (dbPermission != null)
            {
                dbPermission.Type = (CollectionPermission.PermissionType) permission;
                await _repositoryUnitOfWork.SaveChangesAsync();
            }
            else
            {
                dbPermission = new CollectionPermission
                {
                    CollectionId = collectionId,
                    UserId = userId,
                    Type = (CollectionPermission.PermissionType) permission
                };
                await _repositoryUnitOfWork.GetRepository<ICollectionPermissionsRepository>().AddAsync(dbPermission);
                await _repositoryUnitOfWork.SaveChangesAsync();
            }
        }

        public bool TryIncreaseSubmitsCount(long userId, int cnt)
        {
            if (!CheckLimit(userId, cnt)) return false;
            var limitInfo = LimitInfos[userId];
            lock (limitInfo.Lock)
            {
                limitInfo.MinuteCurrent += cnt;
                limitInfo.MonthCurrent += cnt;
                
                // Write montly limit to db.
                var submitLimit = _repositoryUnitOfWork.GetRepository<ISubmitLimitsRepository>().GetByUserIdAsync(userId).Result;
                submitLimit.SubmitsDuringMonth += cnt;
                _repositoryUnitOfWork.SaveChanges();
            }
            return true;
        }

        #region SubmitLimitsChecking
        private class LimitInfo
        {
            public int MinuteCurrent;
            public int MonthCurrent;
            public int MinuteLimit;
            public int MonthLimit;
            public object Lock;
            public DateTime MinuteLimitClearedTime;
            public DateTime MonthLimitClearedTime;
        }
        
        private LimitInfo CreateLimitInfo(long userId)
        {
            // ReSharper disable once InconsistentlySynchronizedField - because this function is called from concurrent dictionary
            var submitLimit = _repositoryUnitOfWork.GetRepository<ISubmitLimitsRepository>().FindByUserIdAsync(userId).Result;
            if (submitLimit == null)
            {
                submitLimit = new SubmitLimit
                {
                    UserId = userId,
                    MinuteLimit = 120,
                    MonthLimit = 120*60*24*31
                };
                // ReSharper disable once InconsistentlySynchronizedField - because this function is called from concurrent dictionary
                _repositoryUnitOfWork.GetRepository<ISubmitLimitsRepository>().AddAsync(submitLimit).Wait();
                // ReSharper disable once InconsistentlySynchronizedField - because this function is called from concurrent dictionary
                _repositoryUnitOfWork.SaveChangesAsync().Wait();
            }
            return new LimitInfo
            {
                MinuteLimitClearedTime = DateTime.Now,
                MonthLimitClearedTime = submitLimit.SubmitsRefreshDate,
                MinuteCurrent = 0,
                MonthCurrent = submitLimit.SubmitsDuringMonth,
                MinuteLimit = submitLimit.MinuteLimit,
                MonthLimit = submitLimit.MonthLimit,
                Lock = new object()
            };
        }

        private void RefreshMinuteLimitIfNeeded(long userId, LimitInfo limitInfo)
        {
            if (limitInfo.MinuteLimitClearedTime + TimeSpan.FromMinutes(1) >= DateTime.Now) return;
            lock (limitInfo.Lock)
            {
                if (limitInfo.MinuteLimitClearedTime + TimeSpan.FromMinutes(1) >= DateTime.Now) return;

                limitInfo.MinuteCurrent = 0;
                
                limitInfo.MinuteLimit = _repositoryUnitOfWork.GetRepository<ISubmitLimitsRepository>().GetByUserIdAsync(userId).Result.MinuteLimit;
                limitInfo.MinuteLimitClearedTime = DateTime.Now;
            }
        }

        private void RefreshMonthLimitIfNeeded(long userId, LimitInfo limitInfo)
        {
            if (limitInfo.MonthLimitClearedTime + TimeSpan.FromDays(31) >= DateTime.Now) return;
            lock (limitInfo.Lock)
            {
                if (limitInfo.MonthLimitClearedTime + TimeSpan.FromDays(31) >= DateTime.Now) return;

                limitInfo.MonthCurrent = 0;

                var submitLimit = _repositoryUnitOfWork.GetRepository<ISubmitLimitsRepository>().GetByUserIdAsync(userId).Result;
                submitLimit.SubmitsRefreshDate = DateTime.Now;
                submitLimit.SubmitsDuringMonth = 0;
                _repositoryUnitOfWork.SaveChangesAsync();
                
                limitInfo.MonthLimit = submitLimit.MonthLimit;
                limitInfo.MonthLimitClearedTime = DateTime.Now;
            }
        }

        private bool CheckLimit(long userId, int submitsCount)
        {
            var limitInfo = LimitInfos.GetOrAdd(userId, CreateLimitInfo);
            RefreshMinuteLimitIfNeeded(userId, limitInfo);
            if (limitInfo.MinuteCurrent + submitsCount > limitInfo.MinuteLimit) return false;
            RefreshMonthLimitIfNeeded(userId, limitInfo);
            return limitInfo.MonthCurrent + submitsCount <= limitInfo.MonthLimit;
        }
        #endregion
    }
}