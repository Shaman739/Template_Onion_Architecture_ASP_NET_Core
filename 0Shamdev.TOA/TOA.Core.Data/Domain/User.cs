using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.ERP.Core.Data.Domain
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class User : DomainObject
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
