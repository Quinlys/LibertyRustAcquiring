using LibertyRustAcquiring.Order.GetOrderData;

namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderDataQuery : IRequest<GetOrderDataResponse>
    {
        public List<int> Packs { get; }
        public GetOrderDataQuery(List<int> packs)
        {
            Packs = packs;    
        }
    }
}
