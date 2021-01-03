using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations
{
    /// <summary>
    /// Фабрика стратегий CRUD операций.
    /// Тут можно удалить или добавить новые, чтобы БЛЛ знала о новый типах или при удалении типа стратегии, БЛЛ не могла выполнить нужный тип CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PrepareItemForCRUDStrategyFactory<TEntity>
        where TEntity : DomainObject, new()
    {
        public PrepareItemForCRUDStrategyFactory(IUnitOfWork uow)
        {

            listStrategies = new Dictionary<ExecuteTypeConstCRUD, Lazy<IPrepareItemForCRUDStrategy<TEntity>>>();
            listStrategies.Add(ExecuteTypeConstCRUD.ADD, new Lazy<IPrepareItemForCRUDStrategy<TEntity>>(() => new AddPrepareItemForCRUDStrategy<TEntity>(uow)));
            listStrategies.Add(ExecuteTypeConstCRUD.EDIT, new Lazy<IPrepareItemForCRUDStrategy<TEntity>>(() => new EditPrepareItemForCRUDStrategy<TEntity>(uow)));
            listStrategies.Add(ExecuteTypeConstCRUD.DELETE, new Lazy<IPrepareItemForCRUDStrategy<TEntity>>(() => new DeletePrepareItemForCRUDStrategy<TEntity>(uow)));

        }
        Dictionary<ExecuteTypeConstCRUD, Lazy<IPrepareItemForCRUDStrategy<TEntity>>> listStrategies;
        /// <summary>
        /// Получение стратегии по типу CRUD
        /// </summary>
        /// <param name="executeType"></param>
        /// <returns></returns>
        public IPrepareItemForCRUDStrategy<TEntity> GetStrategy(ExecuteTypeConstCRUD executeType)
        {
            var strategy = listStrategies.SingleOrDefault(x => x.Key == executeType);
            if (strategy.Value == null)
                throw new Exception($"Неизвестный тип стратегии {executeType} при создании объекта для изменения.");
            else
                return strategy.Value.Value;
        }
        /// <summary>
        ///  Удаление стратегии по типу CRUD
        /// </summary>
        /// <param name="executeType"></param>
        public void RemoveStrategy(ExecuteTypeConstCRUD executeType)
        {
            var strategy = listStrategies.SingleOrDefault(x => x.Key == executeType);
            if (strategy.Value != null) listStrategies.Remove(executeType);
        }
        /// <summary>
        ///  Замена стратегии по типу CRUD на кастомную
        /// </summary>
        /// <param name="executeType"></param>
        /// <param name="newStrategy"></param>
        public void ReplaceStrategy(ExecuteTypeConstCRUD executeType, IPrepareItemForCRUDStrategy<TEntity> newStrategy)
        {
            var strategy = listStrategies.SingleOrDefault(x => x.Key == executeType);
            if (strategy.Value != null) listStrategies.Remove(executeType);
            listStrategies.Add(executeType, new Lazy<IPrepareItemForCRUDStrategy<TEntity>>(() => newStrategy));

        }

        /// <summary>
        /// Подготовка объекта для CRUD.
        /// 1- получение по типу ExecuteTypeCRUD нужной стратегии
        /// 2 - добавление\изменение в контексте на данные с клиента
        /// 3 - отдача результата об успешной подготовки или ошибке(например, по ExecuteTypeCRUD.EDIT не найдена запись в БД)
        /// </summary>
        /// <param name="queryObject"></param>
        /// <param name="executeType"></param>
        /// <returns></returns>
        public BaseResultType<PrepareItemResult<TEntity>> PrepareItem(DefaultParamOfCRUDOperation<TEntity> queryObject, ExecuteTypeConstCRUD executeType)
        {
            BaseResultType<PrepareItemResult<TEntity>> result = new BaseResultType<PrepareItemResult<TEntity>>();
            try
            {
                result.Data.Item = GetStrategy(executeType).GetItem(queryObject);
                result.Status = ResultStatus.Success;
            }
            catch (Exception e)
            {
                result.AddError(e.Message);
            }
            return result;

        }

    }
}
