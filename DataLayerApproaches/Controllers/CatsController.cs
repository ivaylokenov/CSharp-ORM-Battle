namespace DataLayerApproaches.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;

    [ApiController]
    [Route("[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly CatsDbContext data;

        public CatsController(CatsDbContext data) => this.data = data;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatWithOwnerResponseModel>>> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await this.data
                .Cats
                .Where(c => c.Name == name)
                .OrderBy(c => c.Age)
                .Select(c => new CatWithOwnerResponseModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                    Owner = c.Owner.Name
                })
                .ToListAsync();

            if (!result.Any())
            {
                return NotFound();
            }

            return result;
        }

        [HttpPut]
        [Route(nameof(UpdateOwner))]
        public async Task<IActionResult> UpdateOwner(int id, int ownerId)
        {
            var cat = await this.data.Cats.FindAsync(id);

            if (cat == null)
            {
                return BadRequest();
            }

            var ownerExists = await this.data.Owners.AnyAsync(o => o.Id == ownerId);

            if (!ownerExists)
            {
                return BadRequest();
            }

            cat.OwnerId = ownerId;

            await this.data.SaveChangesAsync();

            return Ok();
        }
    }
}

