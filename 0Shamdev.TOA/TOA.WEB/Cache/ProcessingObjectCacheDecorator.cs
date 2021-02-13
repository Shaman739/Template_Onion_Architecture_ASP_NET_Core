using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.WEB.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.TOA.Web.Cache
{
    /// <summary>
    /// Добавляет работу с кешем 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ProcessingObjectCacheDecorator<TEntity> : IProcessingObject<TEntity>
        where TEntity : DomainObject, new()
    {
        IProcessingObject<TEntity> _processingObject;
        ICache<TEntity> _cache;
        public ProcessingObjectCacheDecorator(IProcessingObject<TEntity> processingObject, ICache<TEntity> cache)
        {
            _cache = cache;
            _processingObject = processingObject;
        }
        public IDefaultCRUDBLL<TEntity> GetCRUDHandler()
        {
            IDefaultCRUDBLL<TEntity> defaultCRUDBLL = _processingObject.GetCRUDHandler();
            if (_cache != null)
                defaultCRUDBLL.DomainChangeEvent += new IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity>((executeTypeCRUD, item) =>
                {
                    //Подпись на события БЛЛ
                    if (executeTypeCRUD == ExecuteTypeConstCRUD.ADD)
                        _cache.AddAsync(item);
                    else if (executeTypeCRUD == ExecuteTypeConstCRUD.EDIT)
                        _cache.UpdateAsync(item);
                    else if (executeTypeCRUD == ExecuteTypeConstCRUD.DELETE)
                        _cache.DeleteAsync(item);

                });
            return defaultCRUDBLL;
        }

        public IFetchData<TEntity> GetFetchDataHandler()
        {
            return  new FetchDataCacheDecorator<TEntity>(_processingObject.GetFetchDataHandler(),_cache);
        }
    }
}
