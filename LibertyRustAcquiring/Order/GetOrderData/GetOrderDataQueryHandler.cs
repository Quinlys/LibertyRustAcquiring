using LibertyRustAcquiring.Data;
using LibertyRustAcquiring.Order.GetOrderData;
using LibertyRustAcquiring.Settings;
using LibertyRustAcquiring.Utils;
using Microsoft.EntityFrameworkCore;

namespace LibertyRustAcquiring.Order.GetOrderPrice
{
    public class GetOrderDataQueryHandler(
        ApplicationDbContext context,
        IConfiguration configuration,
        IServerConnection connection,
        ILogger<GetOrderDataQueryHandler> logger) : IRequestHandler<GetOrderDataQuery, GetOrderDataResponse>
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



            var resourcePacks = packs
                .Where(p => p.Type is Models.Enums.PackType.Resource)
                .ToList();

            var totalItems = (from gp in groupedPacks
                              join p in resourcePacks on gp.PackId equals p.Id
                              select p.Items!.Count).Sum();

            bool canBeCreated = await CanCreateAnOrder(request.Server, request.SteamId, totalItems, packs);

            return new GetOrderDataResponse(totalItems, totalPrice, canBeCreated);
        }
        private async Task<bool> CanCreateAnOrder(string server, string steamId, int items, List<Pack> packs)
        {
            var serverInfo = new ServerInfo
            {
                Hostname = configuration[$"{server}:Ip"]! ?? throw new ObjectIsNullException<ServerInfo>(),
                RconPort = configuration[$"{server}:Port"]! ?? throw new ObjectIsNullException<ServerInfo>(),
                RconPassword = configuration[$"{server}:Password"]! ?? throw new ObjectIsNullException<ServerInfo>()
            };

            var online = await CheckPlayerOnline(serverInfo, steamId);
            var slots = await CheckPlayerSlots(serverInfo, steamId, items);

            if (packs.Where(x => x.Type == Models.Enums.PackType.Resource).ToList().Count > 0 && !slots)
            {
                return false;
            }
            else
            {
                bool result = false;
                foreach (var pack in packs)
                {
                    switch (pack.Type)
                    {
                        case Models.Enums.PackType.Resource:
                            result = (online is false || slots is false) ? false : true;
                            break;

                        case Models.Enums.PackType.Blueprints:
                            result = (online is false) ? false : true;
                            break;

                        case Models.Enums.PackType.Privilege:
                            result = true;
                            break;

                        case Models.Enums.PackType.Skins:
                            result = true;
                            break;
                    }
                    if (result is false) return false;
                }
                return true;
            }
                     
        }
        private async Task<bool> CheckPlayerOnline(ServerInfo server, string steamId)
        {
            var checkOnlineCommand = RustCommands.CheckPlayerOnLine(steamId);
            var checkOnline = await connection.SendCommand(server, checkOnlineCommand);

            if (checkOnline.ToLower() is not "true")
            {
                logger.LogError("[GetOrderDataQueryHandler] Player with steamId: {steamId} is offline on server: {server}", steamId, server.Hostname);
                return false;
            }

            return true;
        }
        private async Task<bool> CheckPlayerSlots(ServerInfo server, string steamId, int items)
        {
            var checkSlotsCommand = RustCommands.CheckSlots(steamId);

            var freeSlots = await connection.SendCommand(server, checkSlotsCommand);

            var result = int.TryParse(freeSlots, out var slots);

            if(result == false) return false;

            if (slots < items && items > 0)
            {
                logger.LogError("[GetOrderDataQueryHandler] Player with steamId: {steamId} does not have enough space in the inventory, current slots: {currentSlots}; required number of slots: {requiredSlots}", steamId, int.Parse(freeSlots), items);
                return false;
            }

            return true;
        }
    }
    
}
