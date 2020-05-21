namespace DataLayerApproaches.Handlers.Cats.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class CatsWithOwnersQuery : IRequest<IEnumerable<CatWithOwnersOutputModel>>
    {
        public string Name { get; set; }

        public class CatsWithOwnersQueryHandler : IRequestHandler<CatsWithOwnersQuery, IEnumerable<CatWithOwnersOutputModel>>
        {
            // private readonly ICatService cats;

            private readonly CatsDbContext data;

            public CatsWithOwnersQueryHandler(CatsDbContext data) => this.data = data;

            public async Task<IEnumerable<CatWithOwnersOutputModel>> Handle(
                CatsWithOwnersQuery request, 
                CancellationToken cancellationToken)
                => await this.data
                    .Cats
                    .Where(c => c.Name == request.Name)
                    .OrderBy(c => c.Age)
                        .Select(c => new CatWithOwnersOutputModel
                        {
                        Id = c.Id,
                        Name = c.Name,
                        Age = c.Age,
                        Owner = c.Owner.Name
                    })
                    .ToListAsync(cancellationToken);
    }
    }
}
