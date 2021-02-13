using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.WEB.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.Web.Cache
{
    public class FetchDataCacheDecorator<TEntity> : IFetchData<TEntity>
        where TEntity : DomainObject, new()
    {
        private IFetchData<TEntity> _fetchData;
        private ICache<TEntity> _cache;

        public FetchDataCacheDecorator(IFetchData<TEntity> fetchData, ICache<TEntity> cache)
        {
            _fetchData = fetchData;
            _cache = cache;
        }
        public Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery)
        {
            return _fetchData.FetchDataAsync(paramQuery);
        }

        public Task<BaseResultType<TEntity>> GetByIdAsync(long id)
        {
            BaseResultType<TEntity> dataFromCache = _cache?.GetByIdAsync(id).Result;
            if (dataFromCache?.Status == ResultStatus.Success && dataFromCache?.Data != null)
                return Task.Run<BaseResultType<TEntity>>(() => { 
                    return dataFromCache; 
                });
            else
                return _fetchData.GetByIdAsync(id);
        }
    }
}
