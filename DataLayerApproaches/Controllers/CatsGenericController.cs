namespace DataLayerApproaches.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Models;
    using Data.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;

    [ApiController]
    [Route("[controller]")]
    public class CatsGenericController : ControllerBase
    {
        private readonly IRepository<Cat> cats;
        private readonly IRepository<Owner> owners;

        public CatsGenericController(
            IRepository<Cat> cats, 
            IRepository<Owner> owners)
        {
            this.cats = cats;
            this.owners = owners;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatWithOwnerResponseModel>>> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await this.cats
                .GetAll()
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
            var cat = await this.cats.GetById(id);

            if (cat == null)
            {
                return BadRequest();
            }

            var ownerExists = await this.owners
                .GetAll()
                .AnyAsync(o => o.Id == ownerId);

            if (!ownerExists)
            {
                return BadRequest();
            }

            cat.OwnerId = ownerId;

            await this.cats.Update(cat);

            return Ok();
        }
    }
}
