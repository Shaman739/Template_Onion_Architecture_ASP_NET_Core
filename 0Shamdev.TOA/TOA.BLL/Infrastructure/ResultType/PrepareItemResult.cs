using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System.Collections.Generic;

namespace Shamdev.TOA.BLL.Infrastructure.ResultType
{
    /// <summary>
    /// Подготовка объекта для CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PrepareItemResult<TEntity>
        where TEntity : DomainObject
    {
        /// <summary>
        /// Объекта, который будет сохранен. 
        /// </summary>
        public TEntity Item { get; set; }

        /// <summary>
        /// Вопросы с ответами
        /// </summary>
        public List<Question> Question { get; private set; }
    }
}
