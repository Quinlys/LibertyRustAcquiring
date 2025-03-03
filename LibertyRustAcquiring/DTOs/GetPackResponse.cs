using LibertyRustAcquiring.Models.Enums;

namespace LibertyRustAcquiring.DTOs
{
    public record GetPackResponse(int Id, string Name, string Details, string Image, decimal Price, PackType Type);
}
