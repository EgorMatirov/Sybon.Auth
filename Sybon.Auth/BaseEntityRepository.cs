using System.Threading.Tasks;

namespace Sybon.Auth
{
    public class BaseEntityRepository<TEntity> : IBaseEntityRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly AuthContext Context;

        public BaseEntityRepository(AuthContext context)
        {
            Context = context;
        }

        public Task<TEntity> FindAsync(long key)
        {
            return Context.Set<TEntity>().FindAsync(key);
        }

        public async Task<long> AddAsync(TEntity entity)
        {
            var added = await Context.Set<TEntity>().AddAsync(entity);
            return added.Entity.Id;
        }
        
        public async Task RemoveAsync(long key)
        {
            var entity = await FindAsync(key);
            Context.Remove(entity);
        }
    }
}