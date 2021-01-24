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
    public class ProcessingDomainObject<TEntity> : IProcessingDomainObject<TEntity> where TEntity : DomainObject, new()
    {
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
            return new FetchDomainData<TEntity>(contextDB);
        }
        /// <summary>
        /// Изменения данных в БД
        /// </summary>
        /// <returns></returns>
        public virtual IDefaultCRUDBLL<TEntity> GetCRUDHandler()
        {
            return new DefaultCRUDBLL<TEntity>(contextDB);
        }
    }
}
