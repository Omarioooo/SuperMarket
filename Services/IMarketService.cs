namespace SuperMarket.Services
{
    public interface IMarketService
    {
        Task<Product> CreateProductAsync(ProductDto dto);
        Task<Product?> UpdateProductAsync(int productId, ProductDto dto);
        Task<bool> RemoveProductAsync(int productId);
    }
}