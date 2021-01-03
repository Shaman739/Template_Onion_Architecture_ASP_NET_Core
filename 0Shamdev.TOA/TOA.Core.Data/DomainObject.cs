using System.ComponentModel.DataAnnotations.Schema;

namespace Shamdev.TOA.Core.Data
{
    /// <summary>
    /// Все добенные классы должны быть отнаследованы от этого, чтобы работали БЛЛ и тд
    /// </summary>
    public class DomainObject : ICustomIdentity
    {
        public long Id { get; set; }
        [NotMapped]
        public string CustomIdentity { get; set; }
    }

    public interface ICustomIdentity
    {
        string CustomIdentity { get; set; }
    }
}
