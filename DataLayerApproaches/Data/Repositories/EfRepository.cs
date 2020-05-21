namespace DataLayerApproaches.Data.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;

    // This is an abstraction over an abstraction.
    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly CatsDbContext db;

        public EfRepository(CatsDbContext db) => this.db = db;

        public IQueryable<TEntity> GetAll() => this.db.Set<TEntity>();

        public ValueTask<TEntity> GetById(object id) => this.db.FindAsync<TEntity>(id);

        public async Task Create(TEntity entity)
        {
            this.db.Add(entity);
            await this.db.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            this.db.Update(entity);
            await this.db.SaveChangesAsync();
        }

        public async Task Delete(TEntity entity)
        {
            this.db.Remove(entity);
            await this.db.SaveChangesAsync();
        }
    }
}
