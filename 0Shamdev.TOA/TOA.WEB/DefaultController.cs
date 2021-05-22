using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Infrastructure.Interface;
using Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shamdev.TOA.Web
{
    public class DefaultController<TEntity> : DefaultController<TEntity, FetchDataParameters>
       where TEntity : DomainObject, new()
    {
        public DefaultController(ILogger<DefaultController<TEntity>> logger, IDefaultCRUDBLL<TEntity> defaultCRUDBLL, IFetchData<TEntity> fetchData) : base(logger, defaultCRUDBLL, fetchData)
        {
        }
    }
    public class DefaultController<TEntity, TFetchDataParameters> : Controller
        where TEntity : DomainObject, new()
            where TFetchDataParameters : FetchDataParameters, new()
    {
        private readonly ILogger<DefaultController<TEntity>> _logger;
        IDefaultCRUDBLL<TEntity> _defaultCRUDBLL;
        private IFetchData<TEntity> _fetchData;

        public DefaultController(ILogger<DefaultController<TEntity>> logger, IDefaultCRUDBLL<TEntity> defaultCRUDBLL, IFetchData<TEntity> fetchData)
        {
            _logger = logger;
            _defaultCRUDBLL = defaultCRUDBLL;
            _fetchData = fetchData;
        }

        /// <summary>
        /// Запрос получения данных и БД
        /// </summary>
        /// <param name="fetchDataParameters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetAsync([FromQuery] TFetchDataParameters fetchDataParameters)
        {
            if (fetchDataParameters == null) fetchDataParameters = new TFetchDataParameters();

            ResultFetchData<TEntity> items = await _fetchData.FetchDataAsync(fetchDataParameters);
            BaseResultType resultQuery = new BaseResultType<ResultFetchData<TEntity>>();

            (resultQuery as BaseResultType<ResultFetchData<TEntity>>).Data = items;

            return Json(resultQuery);
        }

        [HttpGet("{id}")]
        [Route("Get")]
        public async Task<JsonResult> GetByIdAsync(long id)
        {
            BaseResultType resultQuery = await _fetchData.GetByIdAsync(id);
          
            return Json(resultQuery);
        }

        [HttpPost]
        public async Task<JsonResult> Add(DefaultParamOfCRUDOperation<TEntity> paramOfCRUD)
        {
            return Json(await _defaultCRUDBLL.SaveItemAsync(ExecuteTypeConstCRUD.ADD, paramOfCRUD));
         }
        [HttpPut]
        public async Task<JsonResult> Edit(DefaultParamOfCRUDOperation<TEntity> paramOfCRUD)
        {
            return Json(await _defaultCRUDBLL.SaveItemAsync(ExecuteTypeConstCRUD.EDIT, paramOfCRUD)); 

        }
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(long id)
        {
            DefaultParamOfCRUDOperation<TEntity> param = new DefaultParamOfCRUDOperation<TEntity>();
            param.Item = new TEntity()
            {
                Id = id
            };
            return Json(await _defaultCRUDBLL.SaveItemAsync(ExecuteTypeConstCRUD.DELETE, param));

        }

    }
}
