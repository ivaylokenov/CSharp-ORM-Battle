namespace DataLayerApproaches.Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseService<TEntity>
        where TEntity : class
    {
        protected BaseService(CatsDbContext data) => this.Data = data;

        protected CatsDbContext Data { get; }

        protected DbSet<TEntity> Set => this.Data.Set<TEntity>();

        protected IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> search = null,
            Expression<Func<TEntity, object>> orderBy = null,
            bool ascending = true)
        {
            var query = this.Set.AsQueryable();

            if (search != null)
            {
                query = query.Where(search);
            }

            if (orderBy != null)
            {
                query = ascending 
                    ? query.OrderBy(orderBy) 
                    : query.OrderByDescending(orderBy);
            }

            return query;
        }
    }
}
