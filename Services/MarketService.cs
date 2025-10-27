namespace SuperMarket.Services
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class MarketService : IMarketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly INotificationService _notificationService;

        public MarketService(IUnitOfWork unitOfWork, IUserContextService userContext,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _notificationService = notificationService;
        }

        // Create New Product
        public async Task<Product> CreateProductAsync(ProductDto model)
        {
            int marketId = _userContext.GetCurrentUserId();

            using MemoryStream stream = new MemoryStream();
            await model.Photo.CopyToAsync(stream);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Photo = stream.ToArray(),
                Price = model.Price
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

            // Notifications Handeler
            var subscribers = await _unitOfWork.Subscriptions
            .Where(s => s.MarketId == marketId)
            .Select(s => s.ClientId)
            .ToListAsync();

            if (subscribers.Any())
            {
                await _notificationService.SendNotificationAsync(
                    senderId: marketId,
                    receiverIds: subscribers,
                    type: NotificationsTypeEnum.NewProduct
                );
            }

            return product;
        }

        // Update Products
        public async Task<Product?> UpdateProductAsync(int productId, ProductDto model)
        {
            int marketId = _userContext.GetCurrentUserId();

            var product = await _unitOfWork.Products.GetAsync(productId);
            if (product == null) return null;

            var marketProduct = await _unitOfWork.MarketProducts
                .FirstOrDefaultAsync(mp => mp.ProductId == productId && mp.MarketId == marketId);

            if (marketProduct == null)
                throw new UnauthorizedAccessException("You cannot edit this product.");

            using MemoryStream stream = new MemoryStream();
            await model.Photo.CopyToAsync(stream);

            product.Name = model.Name;
            product.Description = model.Description;
            product.Photo = stream.ToArray();
            product.Price = model.Price;

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
