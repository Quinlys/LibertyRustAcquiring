using LibertyRustAcquiring.Data;
using LibertyRustAcquiring.Order.GetOrderData;
using Microsoft.EntityFrameworkCore;

namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderDataQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrderDataQuery, GetOrderDataResponse>
    {
        public async Task<GetOrderDataResponse> Handle(GetOrderDataQuery request, CancellationToken cancellationToken)
        {
            var groupedPacks = request.Packs
                .GroupBy(id => id)
                .Select(g => new { PackId = g.Key, Quantity = g.Count(), })
                .ToList();

            var packs = await context.Packs
                .Where(p => request.Packs.Contains(p.Id))
                .Include(p => p.Items)
                .ToListAsync(cancellationToken);

            decimal totalPrice = groupedPacks.Sum(g =>
            {
                var pack = packs.FirstOrDefault(p => p.Id == g.PackId);

                if (pack == null)
                {
                    throw new KeyNotFoundException($"Pack with an Id {g.PackId} was not found.");
                }

                return pack.Price * g.Quantity;
            });

            packs = packs
                .Where(p => p.Type is Models.Enums.PackType.Resource)
                .ToList();

            var totalItems = (from gp in groupedPacks
                              join p in packs on gp.PackId equals p.Id
                              select p.Items!.Count * gp.Quantity).Sum();

            return new GetOrderDataResponse(totalItems, totalPrice);
        }
    }
}
