using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.WEB.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.Web.Cache
{
    /// <summary>
    /// Добавляет работу с кешем 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CacheCRUDBLLDecorator<TEntity> : IDefaultCRUDBLL<TEntity>
        where TEntity : DomainObject, new()
    {
        IDefaultCRUDBLL<TEntity> _defaultCRUDBLL;
        ICache<TEntity> _cache;
        public CacheCRUDBLLDecorator(IDefaultCRUDBLL<TEntity> defaultCRUDBLL, ICache<TEntity> cache)
        {
            _cache = cache;
            _defaultCRUDBLL = defaultCRUDBLL;
            AppyCache();
        }
        public bool IsOnlyAddInContext { get => _defaultCRUDBLL.IsOnlyAddInContext; set => _defaultCRUDBLL.IsOnlyAddInContext = value; }

        public event IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity> DomainChangeEvent
        {
            add
            {
                _defaultCRUDBLL.DomainChangeEvent += value;
            }

            remove
            {
                _defaultCRUDBLL.DomainChangeEvent -= value;
            }
        }
        public void AppyCache()
        {

            if (_cache != null)
                _defaultCRUDBLL.DomainChangeEvent += new IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity>((executeTypeCRUD, item) =>
                {
                    //Подпись на события БЛЛ
                    if (executeTypeCRUD == ExecuteTypeConstCRUD.ADD)
                        _cache.AddAsync(item);
                    else if (executeTypeCRUD == ExecuteTypeConstCRUD.EDIT)
                        _cache.UpdateAsync(item);
                    else if (executeTypeCRUD == ExecuteTypeConstCRUD.DELETE)
                        _cache.DeleteAsync(item);

                });
        }

        public Task<BaseResultType<SaveResultType<TEntity>>> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation)
        {
            return _defaultCRUDBLL.SaveItemAsync(executeTypeCRUD, paramOfCRUDOperation);
        }
    }
}
