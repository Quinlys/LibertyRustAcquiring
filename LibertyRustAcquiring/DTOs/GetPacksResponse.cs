using LibertyRustAcquiring.Models.Enums;

namespace LibertyRustAcquiring.DTOs
{
    public record GetPacksResponse(int Id, string Name, string Description, string Image, decimal Price, PackType Type);

}
