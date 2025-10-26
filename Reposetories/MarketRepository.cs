using SuperMarket.Models;
using System.Linq.Expressions;

namespace SuperMarket.Reposetories
{
    public class MarketRepository : Repository<Market>, IMarketRepository
    {
        public MarketRepository(Context context) : base(context)
        {
        }

        public override async Task<List<Market>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.AppUser)
                .ToListAsync();
        }

        public async Task<List<Market>> GetAllAsync(Expression<Func<Market, bool>>? expression = null)
        {
            var query = _dbSet.Include(m => m.AppUser).AsQueryable();

            if (expression != null)
                query = query.Where(expression);

            return await query.ToListAsync();
        }


        public override async Task<Market?> GetAsync(int id)
        {
            var market = _dbSet
                .Include(m => m.AppUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            return await market;
        }

        public override async Task<Market?> GetAsync(Expression<Func<Market, bool>> expression)
        {
            var market = _dbSet
                .Include(m => m.AppUser)
                .FirstOrDefaultAsync(expression);

            return await market;
        }

        public override async Task<bool> AddAsync(Market market)
        {
            if (market == null)
                return false;

            await _dbSet.AddAsync(market);
            return true;
        }

        public override async Task<bool> Update(Market market)
        {
            if (market == null)
                return false;

            _dbSet.Update(market);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var market = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (market == null)
                return false;

            _dbSet.Remove(market);
            return true;
        }
    }
}
