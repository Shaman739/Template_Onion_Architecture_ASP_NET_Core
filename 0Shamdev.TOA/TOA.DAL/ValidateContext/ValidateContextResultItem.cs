using System.Collections.Generic;

namespace Shamdev.TOA.DAL.ValidateContext
{
    /// <summary>
    /// Результат валидации по оюъекту
    /// </summary>
    public class ValidateContextResultItem
    {
        public ValidateContextResultItem()
        {
            Fields = new List<ValidateContextResultItem>();
        }
        /// <summary>
        /// Имя объекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Поля, которые прошли валидацию по контексту
        /// </summary>
        public List<ValidateContextResultItem> Fields { get; set; }
    }
}
