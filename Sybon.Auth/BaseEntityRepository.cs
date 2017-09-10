using System.Threading.Tasks;

namespace Sybon.Auth
{
    public class BaseEntityRepository<TEntity, TKey> : IBaseEntityRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly AuthContext Context;

        public BaseEntityRepository(AuthContext context)
        {
            Context = context;
        }

        public Task<TEntity> FindAsync(TKey key)
        {
            return Context.Set<TEntity>().FindAsync(key);
        }

        public async Task<TKey> AddAsync(TEntity entity)
        {
            var added = await Context.Set<TEntity>().AddAsync(entity);
            return added.Entity.Id;
        }
        
        public async Task RemoveAsync(TKey key)
        {
            var entity = await FindAsync(key);
            Context.Remove(entity);
        }
    }
}