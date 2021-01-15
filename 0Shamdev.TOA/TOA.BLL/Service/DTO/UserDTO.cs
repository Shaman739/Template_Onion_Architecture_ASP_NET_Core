using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.TOA.BLL.Service.DTO
{
    /// <summary>
    /// Данные пользователя при аторизации
    /// </summary>
    public class UserDTO : DomainObject
    {
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
    }
}
