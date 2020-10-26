using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.BLL.Validate;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL
{
    public class DefaultCRUDBLL<TEntity> : IDefaultCRUDBLL<TEntity>
        where TEntity : DomainObject, new()
    {
        IUnitOfWork _contextDB;
        IRepository<TEntity> _repository;
        /// <summary>
        /// Стратегии CRUD. Через это проперти можно удалить или изменить стратегии.
        /// Например
        ///  PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeCRUD.ADD, new CustomddPrepareItemForCRUDStrategy<TEntity>(uow))
        ///  Делать это нужно в конструкторе
        /// </summary>
        protected Lazy<PrepareItemForCRUDStrategyFactory<TEntity>> PrepareItemForCRUDStrategyFactory;
        protected Lazy<ValidateDomainObjectFactory<TEntity>> ValidateDomainObject;
        private DefaultCRUDBLL()
        {

        }
        public DefaultCRUDBLL(IUnitOfWork contextDB) : this()
        {
            _contextDB = contextDB;
            _repository = _contextDB.Repository<TEntity>();
            PrepareItemForCRUDStrategyFactory = new Lazy<PrepareItemForCRUDStrategyFactory<TEntity>>(() => new PrepareItemForCRUDStrategyFactory<TEntity>(_contextDB));
            ValidateDomainObject = new Lazy<ValidateDomainObjectFactory<TEntity>>(() => new ValidateDomainObjectFactory<TEntity>(_contextDB));
        }
        public async Task<SaveResultType<TEntity>> SaveItemAsync(byte executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation)
        {
            return await Task.Run(() => SaveItem(executeTypeCRUD, paramOfCRUDOperation));

        }
        public SaveResultType<TEntity> SaveItem(byte executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation)
        {
            SaveResultType<TEntity> resultCRUDOpeartion = new SaveResultType<TEntity>();
            try
            {
                if (paramOfCRUDOperation == null || paramOfCRUDOperation.Item == null)
                    throw new Exception("Объект для добавления/изменения не может быть null.");

                PrepareItemResult<TEntity> prepareItemResult = PrepareItemForCRUDStrategyFactory.Value.PrepareItem(paramOfCRUDOperation.Item, executeTypeCRUD);

                if (!prepareItemResult.IsSuccess)
                    resultCRUDOpeartion.AddError(prepareItemResult.Message);
                else
                    resultCRUDOpeartion = SaveContextWithObject(prepareItemResult.Item, executeTypeCRUD);

            }
            catch (Exception e)
            {
                resultCRUDOpeartion.AddError(e.Message);
            }
            return resultCRUDOpeartion;
        }

        /// <summary>
        /// Сохраняет контекст EF с валидациейю Нужен для добавления и изменения объектов.
        /// Валидация через ValidateDomainObject
        /// </summary>
        private SaveResultType<TEntity> SaveContextWithObject(TEntity item, byte executeTypeCRUD)
        {
            SaveResultType<TEntity> saveResultType = new SaveResultType<TEntity>();
            try
            {
                BaseResultType validate = new BaseResultType() { IsSuccess = true };
                //Валидация объекта по типу запроса
                validate.Merge(ValidateDomainObject.Value.GetValidate(item, executeTypeCRUD));

                if (validate.IsSuccess)
                {
                    _contextDB.SaveChanges();
                    saveResultType.IsSuccess = true;
                    saveResultType.Item = item;
                }
                else
                    saveResultType.AddError(validate.Message);

            }
            catch (Exception e)
            {
                saveResultType.AddError(e.Message);
            }
            return saveResultType;
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
        public async Task<PrepareItemResult<TEntity>> GetByIdAsync(long id)
        {
            return await Task.Run(() => GetById(id));
        }
        private PrepareItemResult<TEntity> GetById(long id)
        {
            PrepareItemResult<TEntity> result = new PrepareItemResult<TEntity>();
            result.IsSuccess = true;
            try
            {
                result.Item = _contextDB.Repository<TEntity>().GetById(id);
                if (result.Item == null)
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
