using Shamdev.TOA.Core.Data;
using System.Collections.Generic;

namespace Shamdev.TOA.DAL.Infrastructure
{
    /// <summary>
    /// Результат запроса данных
    /// </summary>
    public class ResultFetchData<TEntity>
        where TEntity : DomainObject
    {
        /// <summary>
        /// Общее количество строк по запросу
        /// </summary>
        public int TotalCountRows { get; set; }
        /// <summary>
        /// Данные по запросу - это страница
        /// </summary>
        public List<TEntity> Items { get; set; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber{get;set;}
    }
}
