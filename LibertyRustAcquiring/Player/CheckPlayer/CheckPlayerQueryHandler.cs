using LibertyRustAcquiring.Settings;
using LibertyRustAcquiring.Utils;

namespace LibertyRustAcquiring.Player.CheckPlayer
{
    public class CheckPlayerQueryHandler(
        IServerConnection connection,
        IConfiguration configuration, 
        ILogger<CheckPlayerQueryHandler> logger) : IRequestHandler<CheckPlayerQuery, CheckPlayerResponse>
    {
        public async Task<CheckPlayerResponse> Handle(CheckPlayerQuery request, CancellationToken cancellationToken)
        {
            var server = new ServerInfo
            {
                Hostname = configuration[$"{request.Server}:Ip"]! ?? throw new ObjectIsNullException<ServerInfo>(),
                RconPort = configuration[$"{request.Server}:Port"]! ?? throw new ObjectIsNullException<ServerInfo>(),
                RconPassword = configuration[$"{request.Server}:Password"]! ?? throw new ObjectIsNullException<ServerInfo>()
            };

            var checkSlotsCommand = RustCommands.CheckSlots(request.SteamId);

            var freeSlots = await connection.SendCommand(server, checkSlotsCommand);

            if(int.Parse(freeSlots) < request.Items && request.Items > 0)
            {
                logger.LogError("[CheckPlayerQueryHandler] Player with steamId: {steamId} does not have enough space in the inventory, current slots: {currentSlots}; required number of slots: {requiredSlots}", request.SteamId, int.Parse(freeSlots), request.Items);
                return new CheckPlayerResponse(false);
            }

            var checkOnlineCommand = RustCommands.CheckPlayerOnLine(request.SteamId);

            var checkOnline = await connection.SendCommand(server, checkOnlineCommand);

            if (checkOnline.ToLower() is not "true")
            {
                logger.LogError("[CheckPlayerQueryHandler] Player with steamId: {steamId} is offline on server: {server}", request.SteamId, request.Server);
                return new CheckPlayerResponse(false);
            }

            return new CheckPlayerResponse(true);
        }
    }
}