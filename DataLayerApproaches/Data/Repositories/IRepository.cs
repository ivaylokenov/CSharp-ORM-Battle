namespace DataLayerApproaches.Data.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;

    // This is an abstraction over an abstraction.
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        ValueTask<TEntity> GetById(object id);

        Task Create(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TEntity id);
    }
}
