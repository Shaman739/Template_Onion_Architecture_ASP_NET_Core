using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;

namespace Shamdev.TOA.DAL.ValidateContext
{
    /// <summary>
    /// Результат валидации контекста
    /// </summary>
    public class ValidateContextResult : BaseResultType
    {
        public ValidateContextResult()
        {
            _listEntity = new List<ValidateContextResultItem>();
        }

        public string Caption { get; internal set; }

        private List<ValidateContextResultItem> _listEntity;
        public void AddEntity(ValidateContextResultItem entity)
        {
            AddError(CreateErrorMessage(entity));
            _listEntity.Add(entity);
        }
        /// <summary>
        /// Формирование сообщения с ошибкой
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string CreateErrorMessage(ValidateContextResultItem entity)
        {
            string messageError = String.Empty;
            if (entity.Fields?.Count > 0)
                messageError = (!String.IsNullOrWhiteSpace(Message) ? Environment.NewLine : "") + $"Не пройдена проверка записи \"{entity.Name}\":";
            else
                messageError = Environment.NewLine + $"Не заполнено значение \"{entity.Name}\".";
            foreach (var field in entity.Fields)
                messageError += CreateErrorMessage(field);

            return messageError;
        }
        /// <summary>
        /// Для доступа писать нужно validate[0], если ValidateContextResult validate) 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ValidateContextResultItem this[int index]
        {
            get { return _listEntity[index]; }
        }
        public int Count => _listEntity.Count;

        // For IEnumerable<DomainObject>
        public IEnumerator<ValidateContextResultItem> GetEnumerator() => _listEntity.GetEnumerator();

    }
}
