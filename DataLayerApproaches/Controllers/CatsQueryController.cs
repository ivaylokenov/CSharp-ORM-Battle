namespace DataLayerApproaches.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Queries;
    using Queries.Cats;

    [ApiController]
    [Route("[controller]")]
    public class CatsQueryController : ControllerBase
    {
        private readonly CatsWithOwnersQuery catsWithOwnersQuery;
        private readonly UpdateCatOwnerQuery updateCatOwnerQuery;

        public CatsQueryController(
            CatsWithOwnersQuery catsWithOwnersQuery, 
            UpdateCatOwnerQuery updateCatOwnerQuery)
        {
            this.catsWithOwnersQuery = catsWithOwnersQuery;
            this.updateCatOwnerQuery = updateCatOwnerQuery;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await this.catsWithOwnersQuery.Execute(name);

            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut]
        [Route(nameof(UpdateOwner))]
        public async Task<IActionResult> UpdateOwner(int id, int ownerId)
        {
            var success = await this.updateCatOwnerQuery.Execute(id, ownerId);

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}