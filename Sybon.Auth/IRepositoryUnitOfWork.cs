using System;
using System.Threading.Tasks;

namespace Sybon.Auth
{
    public interface IRepositoryUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository>();
        void SaveChanges();
        Task SaveChangesAsync();
    }
}