using LibertyRustAcquiring.Packs.GetPack;
using LibertyRustAcquiring.Packs.GetPacks;
using Microsoft.AspNetCore.Mvc;

namespace LibertyRustAcquiring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacksController : ControllerBase
    {
        private readonly ISender _sender;
        public PacksController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? culture = "ua")
        {
            var result = await _sender.Send(new GetPacksQuery(culture));

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string culture = "ua")
        {
            var result = await _sender.Send(new GetPackQuery(id, culture));

            return Ok(result);
        }
    }
}
