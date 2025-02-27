using LibertyRustAcquiring.Settings;
using LibertyRustAcquiring.Utils;

namespace LibertyRustAcquiring.Player.CheckOnLine
{
    public class CheckPlayerOnlineQueryHandler(
        IServerConnection connection,
        IConfiguration configuration) : IRequestHandler<CheckPlayerOnlineQuery, CheckPlayerOnlineResponse>
    {
        public async Task<CheckPlayerOnlineResponse> Handle(CheckPlayerOnlineQuery request, CancellationToken cancellationToken)
        {

            //"Ip": "168.100.161.151",
            //"Port": 28069,
            //"Password": "Leberty"

            var server = new ServerInfo
            {
                Hostname = configuration[$"{request.Server}:Ip"]!,
                RconPort = configuration[$"{request.Server}:Port"]!,
                RconPassword = configuration[$"{request.Server}:Password" ]!,
            };

            var command = RustCommands.CheckPlayerOnLine(request.SteamId);

            var result = await connection.SendCommand(server, command);

            bool response = result.ToLower() is "false" ? false : true;
            
            return new CheckPlayerOnlineResponse(response);
        }
    }
}
