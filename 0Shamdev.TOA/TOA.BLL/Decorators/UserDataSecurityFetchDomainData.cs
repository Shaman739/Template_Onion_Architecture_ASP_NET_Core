using Shamdev.ERP.Core.Data.Interface;
using Shamdev.ERP.DAL.Common.Infrastructure;
using Shamdev.ERP.DAL.Common.Interface;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.Decorators
{
    /// <summary>
    ///  Добавление параметров фильтрации для получние данных только по пользователю
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class UserDataSecurityFetchDomainData<TEntity> : IFetchData<TEntity>
        where TEntity : DomainObject, IDomainObjectIdentity, new()
    {
        private IFetchData<TEntity> _fetchData;
        private IUserContext _userContext;

        public UserDataSecurityFetchDomainData(IFetchData<TEntity> fetchData, IUserContext userContext)
        {
            _fetchData = fetchData;
            _userContext = userContext;

            if (fetchData == null)
                throw new ArgumentNullException("fetchData");
            if (userContext == null)
                throw new ArgumentNullException("userContext");
        }
        public Task<ResultFetchData<TEntity>> FetchDataAsync(IFetchDataParameters paramQuery)
        {
            //Добавляем параметр фильтрации по идентификатору пользователя
            paramQuery.Filters.Add(new WhereDinamicItem(nameof(IDomainObjectIdentity.UserId), TypeFilterEnum.EQUAL, _userContext.GetUserId().ToString()));

            return _fetchData.FetchDataAsync(paramQuery);
        }

        public Task<BaseResultType<TEntity>> GetByIdAsync(long id)
        {
            return _fetchData.GetByIdAsync(id);
        }
    }
}
