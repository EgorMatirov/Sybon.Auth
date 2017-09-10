using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sybon.Auth
{
    [UsedImplicitly]
    public class RepositoryUnitOfWork<TContext> : IRepositoryUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IServiceProvider _services;

        public RepositoryUnitOfWork(TContext context, IServiceProvider services)
        {
            _context = context;
            _services = services;
        }

        public TRepository GetRepository<TRepository>()
        {
            return (TRepository) _services.GetService(typeof(TRepository));
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // todo: write log
                Console.WriteLine(ex);
            }
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // todo: write log
                Console.WriteLine(ex);
            }
        }
    }
}