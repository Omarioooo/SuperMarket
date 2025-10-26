namespace SuperMarket.Reposetories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllWithMarketAsync();
    }
}
