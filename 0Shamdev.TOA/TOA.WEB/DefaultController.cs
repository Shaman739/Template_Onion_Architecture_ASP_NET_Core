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
            BaseResultType resultQuery = new BaseResultType();
            
            try
            {
                if (fetchDataParameters == null) fetchDataParameters = new FetchDataParameters();

                ResultFetchData<TEntity> items = await _defaultCRUDBLL.FetchDataAsync(fetchDataParameters);
                resultQuery = new BaseResultType<ResultFetchData<TEntity>>();

                (resultQuery as BaseResultType<ResultFetchData<TEntity>>).Data = items;

            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { Message = e.Message };
            }
            return Json(resultQuery);
        }

        [HttpGet("{id}")]
        [Route("Get")]
        public async Task<JsonResult> GetByIdAsync(long id)
        {
            BaseResultType resultQuery = new BaseResultType();
            try
            {
                resultQuery= await _defaultCRUDBLL.GetByIdAsync(id);
               // resultQuery.Merge((resultQuery as BaseResultType<TEntity>));
            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { Message = e.Message };
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
            BaseResultType resultQuery = new BaseResultType();
            try
            {
                BaseResultType<SaveResultType<TEntity>> resultCRUDOpeartion = await _defaultCRUDBLL.SaveItemAsync(executeTypeCRUD, paramOfCRUD);
                resultQuery = resultCRUDOpeartion;
                if (resultCRUDOpeartion == null || !resultCRUDOpeartion.IsSuccess)
                    throw new Exception(resultCRUDOpeartion?.Message);

             //   resultQuery = new BaseResultType<SaveResultType<TEntity>>();
              //  (resultQuery as BaseResultType<SaveResultType<TEntity>>).Data = resultCRUDOpeartion;
            }
            catch (Exception e)
            {
                resultQuery = new FailResultQuery() { Message = e.Message };
            }
            return Json(resultQuery);
        }

    }
}
