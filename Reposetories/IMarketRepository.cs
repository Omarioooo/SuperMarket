using System.Linq.Expressions;

namespace SuperMarket.Reposetories
{
    public interface IMarketRepository : IRepository<Market>
    {
        // Filtter with Status
        Task<List<Market>> GetAllAsync(Expression<Func<Market, bool>> expression);
    }
}
