namespace LibertyRustAcquiring.Player.CheckPlayer
{
    public class CheckPlayerQuery : IRequest<CheckPlayerResponse>
    {
        public string SteamId { get; }
        public string Server { get; }
        public int Items { get; }
        public CheckPlayerQuery(string steamId, string server, int items)
        {
            SteamId = steamId;
            Server = server;
            Items = items;
        }
    }
}
