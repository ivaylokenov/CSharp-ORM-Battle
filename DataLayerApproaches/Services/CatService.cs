namespace DataLayerApproaches.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class CatService : BaseService<Cat>, ICatService
    {
        public CatService(CatsDbContext data)
            : base(data)
        {
        }

        public async Task<IEnumerable<CatWithOwnerServiceModel>> GetAllWithOwners(string name)
            => await this
                .GetAll(
                    search: c => c.Name == name,
                    orderBy: c => c.Age)
                .Select(c => new CatWithOwnerServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                    Owner = c.Owner.Name
                })
                .ToListAsync();

        public async Task<bool> UpdateOwner(int catId, int ownerId)
        {
            var cat = await this.Data.Cats.FindAsync(catId);

            if (cat == null)
            {
                return false;
            }

            var ownerExists = await this.Data.Owners.AnyAsync(o => o.Id == ownerId);

            if (!ownerExists)
            {
                return false;
            }

            cat.OwnerId = ownerId;

            await this.Data.SaveChangesAsync();

            return true;
        }
    }
}
