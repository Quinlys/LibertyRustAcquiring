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
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetPacksQuery());

            return Ok(result);
        }
    }
}
