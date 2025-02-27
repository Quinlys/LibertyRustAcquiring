using LibertyRustAcquiring.Data;
using Microsoft.EntityFrameworkCore;

namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderPriceQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrderPriceQuery, decimal>
    {
        public async Task<decimal> Handle(GetOrderPriceQuery request, CancellationToken cancellationToken)
        {
            var groupedPacks = request.Packs
                .GroupBy(id => id)
                .Select(g => new { PackId = g.Key, Quantity = g.Count(), })
                .ToList();

            var packs = await context.Packs
                .Where(p => request.Packs.Contains(p.Id))
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

            return totalPrice;
        }
    }
}
