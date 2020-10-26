namespace Shamdev.TOA.Core.Data.Attribute
{
    /// <summary>
    /// Аттрибут для вывода названия полей и класса при валидации контекста
    /// </summary>
    public class SummaryAttribute : System.Attribute
    {
        protected string summary;

        public SummaryAttribute(string summary)
        {
            this.summary = summary;
        }
        /// <summary>
        /// Сообщение(название клааса, поля и тд)
        /// </summary>
        public string Summary
        {
            get { return summary; }
        }
    }
}
