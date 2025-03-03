using LibertyRustAcquiring.Order.GetOrderData;

namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderDataQuery : IRequest<GetOrderDataResponse>
    {
        public string Server { get; }
        public string SteamId { get; }
        public List<int> Packs { get; }
        public GetOrderDataQuery(string server, string steamId, List<int> packs)
        {
            Server = server;
            SteamId = steamId;
            Packs = packs;    
        }
    }
}
