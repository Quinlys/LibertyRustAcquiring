using LibertyRustAcquiring.Data;
using LibertyRustAcquiring.DTOs.Monobank;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace LibertyRustAcquiring.Order.FindOrderByInvoiceId
{
    public class FindOrderByInvoiceIdQueryHandler(ApplicationDbContext context) : IRequestHandler<FindOrderByInvoiceIdQuery, Models.Entities.Order>
    {        
        public async Task<Models.Entities.Order> Handle(FindOrderByInvoiceIdQuery request, CancellationToken cancellationToken)
        {
            var result = await context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.InvoiceId == request.InvoiceId);

            if(result is null)
            {
                throw new ObjectIsNullException(typeof(Models.Entities.Order).Name);
            }

            return result;
        }
    }
}
