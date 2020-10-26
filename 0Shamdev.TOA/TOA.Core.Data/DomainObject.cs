namespace Shamdev.TOA.Core.Data
{
    /// <summary>
    /// Все добенные классы должны быть отнаследованы от этого, чтобы работали БЛЛ и тд
    /// </summary>
    public class DomainObject
    {
        public long Id { get; set; }
    }
}
