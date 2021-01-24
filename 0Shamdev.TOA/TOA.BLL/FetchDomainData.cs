using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL
{
    /// <summary>
    /// Получение доменных объектов из БД
    /// </summary>
    /// <typeparam name="TEntity">Тип доменного объекта</typeparam>
    public class FetchDomainData<TEntity> : IFetchData<TEntity>
        where TEntity : DomainObject, new()
    {
        IUnitOfWork _contextDB;
        protected IRepository<TEntity> Repository { get; set; }

        private FetchDomainData()
        {

        }
        public FetchDomainData(IUnitOfWork contextDB) : this()
        {
            _contextDB = contextDB;
            Repository = _contextDB.Repository<TEntity>();
        }

        /// <summary>
        /// Получение данных из БД
        /// </summary>
        /// <param name="paramQuery"></param>
        /// <returns></returns>
        public Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery)
        {
            return _contextDB.Repository<TEntity>().FetchDataAsync(paramQuery);
        }
        /// <summary>
        /// Получение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResultType<TEntity>> GetByIdAsync(long id)
        {
            return await Task.Run(() => GetById(id));
        }
        private BaseResultType<TEntity> GetById(long id)
        {
            BaseResultType<TEntity> result = new BaseResultType<TEntity>();
            result.Status = ResultStatus.Success;
            try
            {
                result.Data = _contextDB.Repository<TEntity>().GetById(id);
                if (result.Data == null)
                    result.AddError("Запись не найдена.");

            }
            catch (Exception e)
            {
                result.AddError(e.Message);
            }
            return result;


        }
    }
}
