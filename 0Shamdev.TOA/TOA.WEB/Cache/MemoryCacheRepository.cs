using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shamdev.TOA.WEB.Cache.Interface;
using Microsoft.Extensions.Caching.Memory;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.WEB.Cache
{
    /// <summary>
    /// Реализация кеша в памяти
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MemoryCacheRepository<TEntity> : ICache<TEntity>
        where TEntity : DomainObject, new()
    {
        MemoryCache memoryCache;
        public MemoryCacheRepository()
        {
            if (memoryCache == null)
                memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
        public Task<bool> AddAsync(TEntity item)
        {
            return Task.Run(() =>
            {
                try
                {
                    memoryCache.Set(item.Id, item);
                    return true;
                }
                catch
                {
                    return false;
                }
            });

        }

        public Task<bool> DeleteAsync(TEntity item)
        {
            return Task.Run(() =>
            {
                try
                {
                    memoryCache.Remove(item.Id);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<BaseResultType<TEntity>> GetByIdAsync(long id)
        {
            return Task.Run(() =>
            {
                BaseResultType<TEntity> notFindedResult = new BaseResultType<TEntity>() { Status = ResultStatus.Fail,Data = null };
                try
                {
                    object item = memoryCache.Get(id);
                    if (item is TEntity)
                        return new BaseResultType<TEntity>() { Status = ResultStatus.Success, Data = (item as TEntity) }; 
                    else
                        return notFindedResult;
                }
                catch
                {
                    return notFindedResult;
                }
            });

        }

        public Task<bool> UpdateAsync(TEntity item)
        {
            return this.AddAsync(item);

        }
    }
}
