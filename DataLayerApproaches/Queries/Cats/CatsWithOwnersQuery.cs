namespace DataLayerApproaches.Queries.Cats
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class CatsWithOwnersQuery
    {
        private readonly CatsDbContext data;

        public CatsWithOwnersQuery(CatsDbContext data) => this.data = data;

        public async Task<List<CatWithOwnerServiceModel>> Execute(string name)
            => await this.data
                .Cats
                .Where(c => c.Name == name)
                .OrderBy(c => c.Age)
                .Select(c => new CatWithOwnerServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                    Owner = c.Owner.Name
                })
                .ToListAsync();
    }
}
