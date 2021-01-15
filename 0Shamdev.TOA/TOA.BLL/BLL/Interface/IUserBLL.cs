using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.BLL.Interface
{
    public interface IUserBLL: IDefaultCRUDBLL<User>
    {
        /// <summary>
        /// Проверка возможности авторизации
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<BaseResultType<User>> LoginAllowCheckAsync(DefaultParamOfCRUDOperation<User> param);
    }
}
