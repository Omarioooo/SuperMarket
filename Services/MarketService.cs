namespace SuperMarket.Services
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class MarketService : IMarketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;

        public MarketService(IUnitOfWork unitOfWork, IUserContextService userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        // Create New Product
        public async Task<Product> CreateProductAsync(ProductDto dto)
        {
            int marketId = _userContext.GetCurrentUserId();

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Photo = dto.Photo,
                Price = dto.Price
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();

            var marketProduct = new MarketProduct
            {
                ProductId = product.Id,
                MarketId = marketId
            };

            await _unitOfWork.MarketProducts.AddAsync(marketProduct);
            await _unitOfWork.SaveAsync();

            return product;
        }

        // Update Products
        public async Task<Product?> UpdateProductAsync(int productId, ProductDto dto)
        {
            int marketId = _userContext.GetCurrentUserId();

            var product = await _unitOfWork.Products.GetAsync(productId);
            if (product == null) return null;

            var marketProduct = await _unitOfWork.MarketProducts
                .FirstOrDefaultAsync(mp => mp.ProductId == productId && mp.MarketId == marketId);

            if (marketProduct == null)
                throw new UnauthorizedAccessException("You cannot edit this product.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Photo = dto.Photo;
            product.Price = dto.Price;

            await _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();

            return product;
        }

        // Delete
        public async Task<bool> RemoveProductAsync(int productId)
        {
            int marketId = _userContext.GetCurrentUserId();

            var marketProduct = await _unitOfWork.MarketProducts
                .FirstOrDefaultAsync(mp => mp.ProductId == productId && mp.MarketId == marketId);

            if (marketProduct == null)
                throw new UnauthorizedAccessException("You cannot delete this product.");

            // Delete the FK first
            _unitOfWork.MarketProducts.Remove(marketProduct);

            // Delete the Product
            var product = await _unitOfWork.Products.GetAsync(productId);
            if (product != null)
                await _unitOfWork.Products.DeleteAsync(productId);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }

}
