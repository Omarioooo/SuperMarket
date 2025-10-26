using SuperMarket.Reposetories;
using System.Linq;
using System.Linq.Expressions;

namespace SuperClient.Reposetories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(Context context) : base(context)
        {
        }

        public override async Task<List<Client>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.AppUser)
                .ToListAsync();
        }


        public override async Task<Client?> GetAsync(int id)
        {
            var Client = _dbSet
                .Include(m => m.AppUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            return await Client;
        }

        public override async Task<Client?> GetAsync(Expression<Func<Client, bool>> expression)
        {
            var Client = _dbSet
                .Include(m => m.AppUser)
                .FirstOrDefaultAsync(expression);

            return await Client;
        }

        public override async Task<bool> AddAsync(Client client)
        {
            if (client == null)
                return false;

            await _dbSet.AddAsync(client);
            return true;
        }

        public override async Task<bool> Update(Client client)
        {
            if (client == null)
                return false;

            _dbSet.Update(client);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var Client = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (Client == null)
                return false;

            _dbSet.Remove(Client);
            return true;
        }
    }
}
