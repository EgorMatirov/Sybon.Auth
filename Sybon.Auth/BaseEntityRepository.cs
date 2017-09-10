using System.Threading.Tasks;

namespace Sybon.Auth
{
    public class BaseEntityRepository<TEntity, TKey> : IBaseEntityRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly EntityContext<TEntity> Context;

        public BaseEntityRepository(EntityContext<TEntity> context)
        {
            Context = context;
        }

        public Task<TEntity> FindAsync(TKey key)
        {
            return Context.Entities.FindAsync(key);
        }

        public async Task<TKey> AddAsync(TEntity entity)
        {
            var added = await Context.Entities.AddAsync(entity);
            return added.Entity.Id;
        }

        public void Remove(TEntity entity)
        {
            Context.Remove(entity);
        }

        public Task SaveAsync()
        {
            return Context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TKey key)
        {
            var entity = await FindAsync(key);
            Context.Remove(entity);
        }
    }
}