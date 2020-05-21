namespace DataLayerApproaches.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICatQueryService
    {
        Task<IEnumerable<CatWithOwnerServiceModel>> GetAllWithOwners(string name);

        Task<bool> UpdateOwner(int catId, int ownerId);
    }
}
