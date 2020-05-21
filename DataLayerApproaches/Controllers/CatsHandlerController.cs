namespace DataLayerApproaches.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Handlers.Cats.Commands;
    using Handlers.Cats.Queries;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CatsHandlerController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await this.Mediator.Send(new CatsWithOwnersQuery
            {
                Name = name
            });

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
            var success = await this.Mediator.Send(new UpdateCatOwnerCommand
            {
                CatId = id,
                OwnerId = ownerId
            });

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}