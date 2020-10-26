using Shamdev.TOA.BLL.Infrastructure;
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
            public byte ExecuteType { get; set; }
            public Lazy<IValidateDomainObject<TEntity>> ValidateDomainObject { get; set; }

            public ValidateStrategyType(byte executeType, Lazy<IValidateDomainObject<TEntity>> validateDomainObject)
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
        public BaseResultType GetValidate(TEntity item, byte executeTypeConstCRUD)
        {
            BaseResultType baseResultType = new BaseResultType() { IsSuccess = true };
            byte validateTypeConstCRUD = GetTypeValidateByExecuteType(executeTypeConstCRUD);
            var listValidate = _listValidateType.Where(x => x.ExecuteType == validateTypeConstCRUD).ToList();
            foreach (var validate in listValidate)
            {
                baseResultType.Merge(validate.ValidateDomainObject.Value.Validate(item));
                if (!baseResultType.IsSuccess)
                    break;
            }

            return baseResultType;
        }

        private byte GetTypeValidateByExecuteType(byte executeTypeConstCRUD) =>
             executeTypeConstCRUD switch
             {
                 ExecuteTypeConstCRUD.ADD => ValidateTypeConstCRUD.ADD_OR_EDIT,
                 ExecuteTypeConstCRUD.EDIT => ValidateTypeConstCRUD.ADD_OR_EDIT,
                 ExecuteTypeConstCRUD.DELETE => ValidateTypeConstCRUD.DELETE,
                 _ => throw new ArgumentException("Неизвестный тип валидации.")
             };


        /// <summary>
        ///  Добавление стратегии по типу CRUD
        /// </summary>
        /// <param name="executeTypeConstCRUD">Константа из ValidateTypeConstCRUD</param>
        /// <param name="newStrategy"></param>
        public void AddStrategy(byte executeTypeConstCRUD, Lazy<IValidateDomainObject<TEntity>> newStrategy)
        {
            _listValidateType.Add(
                new ValidateStrategyType(executeTypeConstCRUD, newStrategy)
                );
        }

        //TODO: Добавить описание мапинга по константам или изменить на типы в классе констант
    }
}
