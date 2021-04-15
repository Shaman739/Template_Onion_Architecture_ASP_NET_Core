using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Validate;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL
{
    public class DefaultCRUDBLL<TEntity> : IDefaultCRUDBLL<TEntity>
        where TEntity : DomainObject, new()
    {

        /// <summary>
        /// Событие после успешной операции над объектом
        /// </summary>
        public event IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity> DomainChangeEvent;
        object objectLock = new Object();
        event IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity> IDefaultCRUDBLL<TEntity>.DomainChangeEvent
        {
            add
            {
                lock (objectLock)
                {
                    DomainChangeEvent += value;
                }
            }

            remove
            {
                lock (objectLock)
                {
                    DomainChangeEvent -= value;
                }
            }
        }
        IUnitOfWork _contextDB;

        /// <summary>
        /// Признак, что нужно только добавить в контекст, но не сохранять в БД
        /// </summary>
        public bool IsOnlyAddInContext { get; set; }
        /// <summary>
        /// Стратегии CRUD. Через это проперти можно удалить или изменить стратегии.
        /// Например
        ///  PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeCRUD.ADD, new CustomddPrepareItemForCRUDStrategy<TEntity>(uow))
        ///  Делать это нужно в конструкторе
        /// </summary>
        protected Lazy<IPrepareItemForCRUDStrategyFactory<TEntity>> PrepareItemForCRUDStrategyFactory;
        protected Lazy<ValidateDomainObjectFactory<TEntity>> ValidateDomainObject;
        private DefaultCRUDBLL()
        {

        }
        public DefaultCRUDBLL(IUnitOfWork contextDB) : this()
        {
            _contextDB = contextDB;


            if (_contextDB== null)
                throw new ArgumentNullException("contextDB");

            PrepareItemForCRUDStrategyFactory = new Lazy<IPrepareItemForCRUDStrategyFactory<TEntity>>(() => new PrepareItemForCRUDStrategyFactory<TEntity>(_contextDB));
            ValidateDomainObject = new Lazy<ValidateDomainObjectFactory<TEntity>>(() => new ValidateDomainObjectFactory<TEntity>(_contextDB));
        }

        public async Task<BaseResultType<SaveResultType<TEntity>>> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation)
        {
            BaseResultType<SaveResultType<TEntity>> resultCRUDOpeartion = new BaseResultType<SaveResultType<TEntity>>();
            try
            {
                if (paramOfCRUDOperation == null || paramOfCRUDOperation.Item == null)
                    throw new Exception("Объект для добавления/изменения не может быть null.");

                BaseResultType<PrepareItemResult<TEntity>> prepareItemResult = PrepareItemForCRUDStrategyFactory.Value.PrepareItem(paramOfCRUDOperation, executeTypeCRUD);
              
                DefaultParamOfCRUDOperation<TEntity> paramValidate = new DefaultParamOfCRUDOperation<TEntity>();
                paramOfCRUDOperation.Item = prepareItemResult.Data.Item;
                paramOfCRUDOperation.Questions = paramOfCRUDOperation.Questions;
               
                if (prepareItemResult.Status == ResultStatus.Fail)
                    resultCRUDOpeartion.AddError(prepareItemResult.Message);
                else
                    resultCRUDOpeartion = await SaveContextWithObjectAsync(paramOfCRUDOperation, executeTypeCRUD);

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
        private async Task<BaseResultType<SaveResultType<TEntity>>> SaveContextWithObjectAsync(DefaultParamOfCRUDOperation<TEntity> item, ExecuteTypeConstCRUD executeTypeCRUD)
        {
            BaseResultType<SaveResultType<TEntity>> saveResultType = new BaseResultType<SaveResultType<TEntity>>();
            try
            {
                BaseResultType validate = new BaseResultType() { Status = ResultStatus.Success };
                //Валидация объекта по типу запроса
                validate.Merge(ValidateDomainObject.Value.GetValidate(item, executeTypeCRUD));

                saveResultType.Merge(validate);

                if (validate.Status == ResultStatus.Success)
                {
                    if (!IsOnlyAddInContext)
                       await _contextDB.SaveChangesAsync();

                    saveResultType.Status = ResultStatus.Success;
                    saveResultType.Data.Item = item.Item;
                    //Оповещаем об успешном изменение
                    if (DomainChangeEvent != null) DomainChangeEvent(executeTypeCRUD,item.Item);
                }
               // else if (validate.Status == ResultStatus.Fail)
               // {
               //     saveResultType.AddError(validate.Message);
              //  }
                if (saveResultType.Question?.Count > 0)
                    saveResultType.Data.Item = item.Item;

            }
            catch (Exception e)
            {
                saveResultType.AddError(e.Message);
            }
            return saveResultType;
        }

       
    }
}
