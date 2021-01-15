using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Domain
{
    /// <summary>
    /// Улица
    /// </summary>
    public class Street : DomainObject
    { 
        /// <summary>
        /// Название улицы
        /// </summary>
        public string Name { get; set; }
    }
}
