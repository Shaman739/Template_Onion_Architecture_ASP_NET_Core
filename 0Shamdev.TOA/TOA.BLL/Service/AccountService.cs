using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL.BLL;
using Shamdev.TOA.BLL.BLL.Interface;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.BLL.Service.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.Service
{
    /// <summary>
    /// Бизнес-логика по работе с учетными данными
    /// </summary>
    public class AccountService : IAccountService
    {
        private IUnitOfWork _contextDB;
        private IUserContext _userContext;
        private IUserBLL _userFetchData;
        private IDefaultCRUDBLL<User> _userBLL;
        private AccountService()
        {

        }
        public AccountService(IUnitOfWork contextDB, IUserContext userContext)
        {
            _contextDB = contextDB;
            _userContext = userContext;
            if (_contextDB == null)
                throw new ArgumentNullException("Отсутствует обязательный параметр contextDB");

            _userBLL = new DefaultCRUDBLL<User>(contextDB);
            _userFetchData = new UserFetchData(contextDB);
        }
        /// <summary>
        /// Проверка возможности авторизации пользователя
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<BaseResultType<User>> LoginAllowCheckAsync(DefaultParamOfCRUDOperation<UserDTO> param)
        {
            DefaultParamOfCRUDOperation<User> paramUser = new DefaultParamOfCRUDOperation<User>();
            paramUser.Item = new User()
            {
                Email = param?.Item?.Email,
                Password = param?.Item?.Password
            };
            return _userFetchData.LoginAllowCheckAsync(paramUser);
           
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<BaseResultType<SaveResultType<User>>> RegisterAsync(DefaultParamOfCRUDOperation<UserDTO> param)
        {
            DefaultParamOfCRUDOperation<User> paramUser = new DefaultParamOfCRUDOperation<User>();
            paramUser.Item = new User()
            {
                Email = param?.Item?.Email,
                Password = param?.Item?.Password
            };
            return _userBLL.SaveItemAsync(ExecuteTypeConstCRUD.ADD, paramUser);
        }
    }
}
