using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.Core.Data.Interface
{
    public interface IDomainObjectIdentity
    {
        /// <summary>
        /// Идентификатор пользователя, чья запись
        /// </summary>
        long? UserId { get; set; }
    }
}
