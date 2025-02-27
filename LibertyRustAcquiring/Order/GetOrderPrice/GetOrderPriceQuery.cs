namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderPriceQuery : IRequest<decimal>
    {
        public List<int> Packs { get; }
        public GetOrderPriceQuery(List<int> packs)
        {
            Packs = packs;    
        }
    }
}
