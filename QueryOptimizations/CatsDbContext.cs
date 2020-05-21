namespace QueryOptimizations
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class CatsDbContext : DbContext
    {
        private readonly bool enableLazyLoading;

        public CatsDbContext(bool enableLazyLoading = false)
            => this.enableLazyLoading = enableLazyLoading;

        public DbSet<Cat> Cats { get; set; }

        public DbSet<Owner> Owners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder
                .UseLazyLoadingProxies(this.enableLazyLoading)
                .UseSqlServer(Settings.ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
            => modelBuilder
                .Entity<Cat>()
                .HasOne(c => c.Owner)
                .WithMany(o => o.Cats)
                .HasForeignKey(c => c.OwnerId);
    }
}
