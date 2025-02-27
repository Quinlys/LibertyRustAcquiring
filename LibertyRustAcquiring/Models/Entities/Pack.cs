using LibertyRustAcquiring.Models.Enums;

namespace LibertyRustAcquiring.Models.Entities
{

    public class Pack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameENG { get; set; }
        public string Details { get; set; }
        public string DetailsENG { get; set; }
        public string Description { get; set; }
        public string DescriptionENG { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public PackType Type { get; set; }

        public List<PackItem>? Items { get; set; }
    }
}
