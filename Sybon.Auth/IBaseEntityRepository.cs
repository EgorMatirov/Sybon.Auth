using System.Threading.Tasks;

namespace Sybon.Auth
{
    public interface IBaseEntityRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        Task<TEntity> FindAsync(TKey key);
        Task<TKey> AddAsync(TEntity entity);
        void Remove(TEntity entity);
        Task SaveAsync();
        Task RemoveAsync(TKey key);
    }
}