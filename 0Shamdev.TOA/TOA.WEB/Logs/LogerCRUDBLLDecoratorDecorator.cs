using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.MongoDB.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data.MongoDB;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shamdev.TOA.Web.Cache
{
    /// <summary>
    /// Добавляет работу с кешем 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class LogerCRUDBLLDecoratorDecorator<TEntity> : IDefaultCRUDBLL<TEntity>
        where TEntity : DomainObject, new()
    {
        IDefaultCRUDBLL<TEntity> _defaultCRUDBLL;
        ILog _log;
        public LogerCRUDBLLDecoratorDecorator(IDefaultCRUDBLL<TEntity> defaultCRUDBLL, ILog log)
        {
            _log = log;
            _defaultCRUDBLL = defaultCRUDBLL;
            ApplyLog();
        }

        public bool IsOnlyAddInContext { get => _defaultCRUDBLL.IsOnlyAddInContext; set => _defaultCRUDBLL.IsOnlyAddInContext = value; }

        public event IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity> DomainChangeEvent
        {
            add
            {
                _defaultCRUDBLL.DomainChangeEvent += value;
            }

            remove
            {
                _defaultCRUDBLL.DomainChangeEvent -= value;
            }
        }

        public void ApplyLog()
        {

            if (_log != null)
                _defaultCRUDBLL.DomainChangeEvent += new IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity>((executeTypeCRUD, item) =>
                {
                    string actionData = JsonSerializer.Serialize(item);
                    string executeType = executeTypeCRUD.Value.ToString();

                    LogItem logItem = new LogItem(actionData, executeType);

                    _log.AddLog(logItem);


                });
        }

        public Task<BaseResultType<SaveResultType<TEntity>>> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation)
        {
            return _defaultCRUDBLL.SaveItemAsync(executeTypeCRUD, paramOfCRUDOperation);
        }
    }
}
