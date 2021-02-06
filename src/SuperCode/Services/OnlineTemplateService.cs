using FreeSql;
using SuperCode.Entities;
using SuperCode.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Services
{
    public interface IOnlineTemplateService
    {
        ValueTask<int> DeleteAsync(params long[] ids);

        Task<List<OnlineTemplateToolEntity>> GetListAsync(string key);

        Task<long> AddAsync(OnlineTemplateToolEntity entity);

        Task<int> UpdateAsync(OnlineTemplateToolEntity entity);
    }

    public class OnlineTemplateService : IOnlineTemplateService
    {
        private readonly IFreeSql _freeSql;
        IBaseRepository<OnlineTemplateToolEntity> _onlineTemplateRepository;
        public OnlineTemplateService(IFreeSql freeSql)
        {
            _freeSql = freeSql;
            _onlineTemplateRepository = freeSql.GetRepository<OnlineTemplateToolEntity>();
        }

        public async Task<long> AddAsync(OnlineTemplateToolEntity entity)
        {
            return (await _onlineTemplateRepository.InsertAsync(entity)).Id;
        }

        public async Task<int> UpdateAsync(OnlineTemplateToolEntity entity)
        {
            return await _onlineTemplateRepository.UpdateAsync(entity);
        }

        public async ValueTask<int> DeleteAsync(params long[] ids)
        {
            return await _onlineTemplateRepository.DeleteAsync(a => ids.Contains(a.Id));
        }

        public async Task<List<OnlineTemplateToolEntity>> GetListAsync(string key)
        {
            return await _onlineTemplateRepository.WhereIf(key.NotNull(), a => a.Name.Contains(key)).ToListAsync();
        }
    }
}
