using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.Service.Interface
{
    public interface IAccountService
    {
        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<BaseResultType<User>> LoginAllowCheckAsync(DefaultParamOfCRUDOperation<UserDTO> param);
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<BaseResultType<SaveResultType<User>>> RegisterAsync(DefaultParamOfCRUDOperation<UserDTO> param);
    }
}
