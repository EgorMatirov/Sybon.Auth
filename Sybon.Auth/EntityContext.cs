using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sybon.Auth
{
    [UsedImplicitly]
    public class EntityContext<TEntity> : DbContext where TEntity : class
    {
        public DbSet<TEntity> Entities { get; [UsedImplicitly] set; }

        public EntityContext([NotNull] DbContextOptions<EntityContext<TEntity>> options) : base(options)
        {
        }
    }
}