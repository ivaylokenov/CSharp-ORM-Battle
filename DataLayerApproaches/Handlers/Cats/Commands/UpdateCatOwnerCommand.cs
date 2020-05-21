namespace DataLayerApproaches.Handlers.Cats.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class UpdateCatOwnerCommand : IRequest<bool>
    {
        public int CatId { get; set; }

        public int OwnerId { get; set; }

        public class UpdateCatOwnerCommandHandler : IRequestHandler<UpdateCatOwnerCommand, bool>
        {
            private readonly CatsDbContext data;

            public UpdateCatOwnerCommandHandler(CatsDbContext data) => this.data = data;

            public async Task<bool> Handle(
                UpdateCatOwnerCommand request, 
                CancellationToken cancellationToken)
            {
                var cat = await this.data.Cats.FindAsync(request.CatId);

                if (cat == null)
                {
                    return false;
                }

                var ownerExists = await this.data.Owners.AnyAsync(o => o.Id == request.OwnerId, cancellationToken);

                if (!ownerExists)
                {
                    return false;
                }

                cat.OwnerId = request.OwnerId;

                await this.data.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
