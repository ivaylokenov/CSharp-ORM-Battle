namespace DataLayerApproaches.Queries.Cats
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class UpdateCatOwnerQuery 
    {
        private readonly CatsDbContext data;

        public UpdateCatOwnerQuery(CatsDbContext data) => this.data = data;

        public async Task<bool> Execute(int catId, int ownerId)
        {
            var cat = await this.data.Cats.FindAsync(catId);

            if (cat == null)
            {
                return false;
            }

            var ownerExists = await this.data.Owners.AnyAsync(o => o.Id == ownerId);

            if (!ownerExists)
            {
                return false;
            }

            cat.OwnerId = ownerId;

            await this.data.SaveChangesAsync();

            return true;
        }
    }
}
