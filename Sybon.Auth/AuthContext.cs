using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Auth.Repositories.UsersRepository.Entities;

namespace Sybon.Auth
{
    public class AuthContext : DbContext
    {
        public AuthContext()
        {
        }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public DbSet<SubmitLimit> SubmitLimits { get; [UsedImplicitly] set; }
        public DbSet<Token> Tokens { get; [UsedImplicitly] set; }
        public DbSet<User> Users { get; [UsedImplicitly] set; }
        public DbSet<CollectionPermission> CollectionPermissions { get; [UsedImplicitly] set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(u => u.ProblemPermissions)
                .WithOne(pp => pp.User)
                .HasForeignKey(pp => pp.UserId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Token)
                .WithOne(t => t.User);
            modelBuilder.Entity<User>()
                .HasOne(u => u.SubmitLimit)
                .WithOne(sl => sl.User);
        }
    }
}