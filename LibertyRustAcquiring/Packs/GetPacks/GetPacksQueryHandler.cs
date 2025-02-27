using LibertyRustAcquiring.Data;
using Microsoft.EntityFrameworkCore;

namespace LibertyRustAcquiring.Packs.GetPacks
{
    public class GetPacksQueryHandler(ApplicationDbContext context) : IRequestHandler<GetPacksQuery, List<Pack>>
    {
        public async Task<List<Pack>> Handle(GetPacksQuery request, CancellationToken cancellationToken)
        {
            return await context.Packs
                .AsNoTracking()
                .Include(x => x.Items)
                .ToListAsync();
        }
    }
}
