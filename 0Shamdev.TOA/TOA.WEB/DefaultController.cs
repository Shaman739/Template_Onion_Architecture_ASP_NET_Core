using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;

using Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery;
using System;
using System.Threading.Tasks;

namespace Shamdev.TOA.Web
{
    public class DefaultController<TEntity> : Controller
        where TEntity : DomainObject, new()
    {
        private readonly ILogger<DefaultController<TEntity>> _logger;
        IDefaultCRUDBLL<TEntity> _defaultCRUDBLL;
        public DefaultController(ILogger<DefaultController<TEntity>> logger, IDefaultCRUDBLL<TEntity> defaultCRUDBLL)
        {
            _logger = logger;
            _defaultCRUDBLL = defaultCRUDBLL;
        }


        /// <summary>
        /// Запрос получения данных и БД
        /// </summary>
        /// <param name="fetchDataParameters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetAsync([FromQuery] FetchDataParameters fetchDataParameters)
        {
            BaseResultType resultQuery;
            try
            {
                FetchDataResultQuery<TEntity> resultSuccessQuery = new FetchDataResultQuery<TEntity>();

                if (fetchDataParameters == null) fetchDataParameters = new FetchDataParameters();

                ResultFetchData<TEntity> items = await _defaultCRUDBLL.FetchDataAsync(fetchDataParameters);
                resultSuccessQuery.Items = items.Items;
                resultSuccessQuery.Count = items.TotalCountRows;
                resultQuery = resultSuccessQuery;
            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { ErrorMessage = e.Message };
            }
            return Json(resultQuery);
        }

        [HttpGet("{id}")]
        [Route("Get")]
        public async Task<JsonResult> GetByIdAsync(long id)
        {
            BaseResultType resultQuery;
            try
            {
                resultQuery = await _defaultCRUDBLL.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { ErrorMessage = e.Message };
            }
            return Json(resultQuery);
        }

        [HttpPost]
        public async Task<JsonResult> Add(DefaultParamOfCRUDOperation<TEntity> paramOfCRUD)
        {
            return await SaveItemAsync(ExecuteTypeConstCRUD.ADD, paramOfCRUD);

        }
        [HttpPut]
        public async Task<JsonResult> Edit(DefaultParamOfCRUDOperation<TEntity> paramOfCRUD)
        {
            return await SaveItemAsync(ExecuteTypeConstCRUD.EDIT, paramOfCRUD);

        }
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(long id)
        {
            DefaultParamOfCRUDOperation<TEntity> param = new DefaultParamOfCRUDOperation<TEntity>();
            param.Item = new TEntity()
            {
                Id = id
            };
            return await SaveItemAsync(ExecuteTypeConstCRUD.DELETE, param);

        }

        private async Task<JsonResult> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUD)
        {
            BaseResultType resultQuery;
            try
            {
                SaveResultType<TEntity> resultCRUDOpeartion = await _defaultCRUDBLL.SaveItemAsync(executeTypeCRUD, paramOfCRUD);

                if (resultCRUDOpeartion == null || !resultCRUDOpeartion.IsSuccess)
                    throw new Exception(resultCRUDOpeartion?.Message);

                resultQuery = resultCRUDOpeartion;
            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { ErrorMessage = e.Message };
            }
            return Json(resultQuery);
        }

    }
}
