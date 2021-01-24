using Shamdev.ERP.Core.Data.Domain;
using Shamdev.ERP.DAL.Common.Repository.Interface;
using Shamdev.TOA.BLL.BLL.Interface;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.BLL
{
    internal class UserFetchData : FetchDomainData<User>, IUserBLL
    {
        public UserFetchData(IUnitOfWork contextDB) : base(contextDB)
        {
        }

        public Task<BaseResultType<User>> LoginAllowCheckAsync(DefaultParamOfCRUDOperation<User> param)
        {
            return Task.Run(() =>
            {
                BaseResultType<User> resultType = new BaseResultType<User>();
                if (param is null || param.Item is null)
                    resultType.AddError("Отсутствует параметр проверки пользователя.");
                else
                {
                    if (!(Repository is IUserRepository)) resultType.AddError("Репозиторий не является IUserRepository");

                    if (String.IsNullOrWhiteSpace(param.Item.Password)) resultType.AddError("Отсутствует пароль.");
                    if (String.IsNullOrWhiteSpace(param.Item.Email)) resultType.AddError("Отсутствует Email.");
                   
                    resultType.Data = (Repository as IUserRepository).CheckIssueUserAsync(param.Item.Email, param.Item.Password).Result;

                    if (resultType.Data is null) resultType.AddError("Позователь не найден.");
                }
                return resultType;
            });

        }
    }
}
