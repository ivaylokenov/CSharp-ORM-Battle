namespace DataLayerApproaches.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Queries.Cats;

    public class CatQueryService : ICatQueryService
    {
        private readonly CatsWithOwnersQuery catsWithOwnersQuery;
        private readonly UpdateCatOwnerQuery updateCatOwnerQuery;

        public CatQueryService(
            CatsWithOwnersQuery catsWithOwnersQuery, 
            UpdateCatOwnerQuery updateCatOwnerQuery)
        {
            this.catsWithOwnersQuery = catsWithOwnersQuery;
            this.updateCatOwnerQuery = updateCatOwnerQuery;
        }

        public async Task<IEnumerable<CatWithOwnerServiceModel>> GetAllWithOwners(string name)
            => await this.catsWithOwnersQuery.Execute(name);

        public async Task<bool> UpdateOwner(int catId, int ownerId)
            => await this.updateCatOwnerQuery.Execute(catId, ownerId);
    }
}
