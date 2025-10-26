

using System.Linq.Expressions;

namespace SuperMarket.Reposetories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(Context context) : base(context)
        {
        }

        public override async Task<List<Product>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<Product>> GetAllWithMarketAsync()
        {
            return await _dbSet
               .Include(p => p.MarketProducts)
               .ThenInclude(mp => mp.Market)
               .ToListAsync();
        }

        public override async Task<Product?> GetAsync(int id)
        {
            var product = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return null;

            return product;
        }

        public override async Task<Product?> GetAsync(Expression<Func<Product, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public override async Task<bool> AddAsync(Product product)
        {
            if (product == null)
                return await Task.FromResult(false);

            _dbSet.Add(product);
            return await Task.FromResult(true);
        }

        public override async Task<bool> Update(Product product)
        {
            if (product == null)
                return await Task.FromResult(false);

            _dbSet.Update(product);
            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var product = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

            if (product == null)
                return await Task.FromResult(false);

            _dbSet.Remove(product);
            return await Task.FromResult(true);
        }

    }
}
