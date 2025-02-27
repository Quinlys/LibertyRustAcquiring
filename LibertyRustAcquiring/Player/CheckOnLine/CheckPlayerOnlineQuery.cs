namespace LibertyRustAcquiring.Player.CheckOnLine
{
    public class CheckPlayerOnlineQuery : IRequest<CheckPlayerOnlineResponse>
    {
        public string SteamId { get; }
        public string Server { get; }
        public CheckPlayerOnlineQuery(string steamId, string server)
        {
            SteamId = steamId;
            Server = server;
        }
    }
}
