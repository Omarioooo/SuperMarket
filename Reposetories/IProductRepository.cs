namespace SuperMarket.Reposetories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllWithMarketAsync();
    }
}
