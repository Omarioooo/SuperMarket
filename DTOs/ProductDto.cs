namespace SuperMarket.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public byte[]? Photo { get; set; }
        public double Price { get; set; }
    }

}
