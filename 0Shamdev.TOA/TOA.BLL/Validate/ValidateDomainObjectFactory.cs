using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shamdev.TOA.BLL.Validate
{
    /// <summary>
    /// Валидация объекта перед сохранением
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ValidateDomainObjectFactory<TEntity>
        where TEntity : DomainObject, new()
    {
        struct ValidateStrategyType
        {
            public ValidateTypeConstCRUD ExecuteType { get; set; }
            public Lazy<IValidateDomainObject<TEntity>> ValidateDomainObject { get; set; }

            public ValidateStrategyType(ValidateTypeConstCRUD executeType, Lazy<IValidateDomainObject<TEntity>> validateDomainObject)
            {
                ExecuteType = executeType;
                ValidateDomainObject = validateDomainObject;
            }
        }
        private IUnitOfWork _contextDB;
        List<ValidateStrategyType> _listValidateType;
        public ValidateDomainObjectFactory(IUnitOfWork contextDB)
        {
            _contextDB = contextDB;
            _listValidateType = new List<ValidateStrategyType>();
            _listValidateType.Add(
                new ValidateStrategyType(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<TEntity>>(() => new ValidateDomainObjectByDBContext<TEntity>(_contextDB)))
                );
        }
        /// <summary>
        /// Проверка объекта перед сохранением
        /// </summary>
        /// <param name="item"></param>
        /// <param name="executeTypeConstCRUD">константа из ExecuteTypeConstCRUD. Она сопоставляется с типами валидации из ValidateTypeConstCRUD</param>
        /// ExecuteTypeConstCRUD.ADD и ExecuteTypeConstCRUD.EDIT => ValidateTypeConstCRUD.ADD_OR_EDIT
        /// ExecuteTypeConstCRUD.DELETE  => ValidateTypeConstCRUD.DELETE
        /// <returns></returns>
        public BaseResultType GetValidate(DefaultParamOfCRUDOperation<TEntity> item, ExecuteTypeConstCRUD executeTypeConstCRUD)
        {
            BaseResultType baseResultType = new BaseResultType() { Status = ResultStatus.Success };
            ValidateTypeConstCRUD validateTypeConstCRUD = GetTypeValidateByExecuteType(executeTypeConstCRUD);
            var listValidate = _listValidateType.Where(x => x.ExecuteType == validateTypeConstCRUD).ToList();
            foreach (var validate in listValidate)
            {
                baseResultType.Merge(validate.ValidateDomainObject.Value.Validate(item));
                if (baseResultType.Status == ResultStatus.Fail)
                    break;
            }

            return baseResultType;
        }

        private ValidateTypeConstCRUD GetTypeValidateByExecuteType(ExecuteTypeConstCRUD executeTypeConstCRUD) {
            if (executeTypeConstCRUD == ExecuteTypeConstCRUD.ADD || executeTypeConstCRUD == ExecuteTypeConstCRUD.EDIT)
                return ValidateTypeConstCRUD.ADD_OR_EDIT;
            else if (executeTypeConstCRUD == ExecuteTypeConstCRUD.DELETE)
                return ValidateTypeConstCRUD.DELETE;
            else
                throw new ArgumentException("Неизвестный тип валидации.");
        }


        /// <summary>
        ///  Добавление стратегии по типу CRUD
        /// </summary>
        /// <param name="executeTypeConstCRUD">Константа из ValidateTypeConstCRUD</param>
        /// <param name="newStrategy"></param>
        public void AddStrategy(ValidateTypeConstCRUD executeTypeConstCRUD, Lazy<IValidateDomainObject<TEntity>> newStrategy)
        {
            _listValidateType.Add(
                new ValidateStrategyType(executeTypeConstCRUD, newStrategy)
                );
        }

        //TODO: Добавить описание мапинга по константам или изменить на типы в классе констант
    }
}
