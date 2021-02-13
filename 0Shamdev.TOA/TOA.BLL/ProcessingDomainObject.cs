using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.TOA.BLL
{
    /// <summary>
    /// Обработка доменных объектов
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ProcessingDomainObject<TEntity> : IProcessingObject<TEntity> where TEntity : DomainObject, new()
    {
        private IFetchData<TEntity> _fetchDataHandler;
        private IDefaultCRUDBLL<TEntity> _crudHandler;
        protected IUnitOfWork contextDB;
        private ProcessingDomainObject()
        {

        }
        
        public ProcessingDomainObject(IUnitOfWork contextDB) : this()
        {
            this.contextDB = contextDB;
        }
        /// <summary>
        /// Получение данных из БД
        /// </summary>
        /// <returns></returns>
        public virtual IFetchData<TEntity> GetFetchDataHandler()
        {
            if(_fetchDataHandler == null)
                _fetchDataHandler = new FetchDomainData<TEntity>(contextDB);

            return _fetchDataHandler;
        }
        /// <summary>
        /// Изменения данных в БД
        /// </summary>
        /// <returns></returns>
        public virtual IDefaultCRUDBLL<TEntity> GetCRUDHandler()
        {
            if (_crudHandler == null)
                _crudHandler = new DefaultCRUDBLL<TEntity>(contextDB);

            return _crudHandler;

        }
    }
}
