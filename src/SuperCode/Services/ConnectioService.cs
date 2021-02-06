using FreeSql;
using SuperCode.Entities;
using SuperCode.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Services
{
    public interface IConnectionService
    {
        ValueTask<int> DeleteAsync(params long[] ids);

        Task<List<ConnectionEntity>> GetListAsync(string key);

        Task<long> AddAsync(ConnectionEntity entity);

        Task<int> UpdateAsync(ConnectionEntity entity);
    }

    public class ConnectionService : IConnectionService
    {
        private readonly IFreeSql _freeSql;
        IBaseRepository<ConnectionEntity> _connectionRepository;
        public ConnectionService(IFreeSql freeSql)
        {
            _freeSql = freeSql;
            _connectionRepository = freeSql.GetRepository<ConnectionEntity>();
        }

        public async Task<long> AddAsync(ConnectionEntity entity)
        {
            return (await _connectionRepository.InsertAsync(entity)).Id;
        }

        public async Task<int> UpdateAsync(ConnectionEntity entity)
        {
            return await _connectionRepository.UpdateAsync(entity);
        }

        public async ValueTask<int> DeleteAsync(params long[] ids)
        {
            return await _connectionRepository.DeleteAsync(a => ids.Contains(a.Id));
        }

        public async Task<List<ConnectionEntity>> GetListAsync(string key)
        {
            return await _connectionRepository.WhereIf(key.NotNull(), a => a.ConnectionName.Contains(key)).ToListAsync();
        }
    }
}
