namespace DataLayerApproaches.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    [ApiController]
    [Route("[controller]")]
    public class CatsServiceController : ControllerBase
    {
        private readonly ICatService cats;

        public CatsServiceController(ICatService cats)
            => this.cats = cats;

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await this.cats.GetAllWithOwners(name);

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
            var success = await this.cats.UpdateOwner(id, ownerId);

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
