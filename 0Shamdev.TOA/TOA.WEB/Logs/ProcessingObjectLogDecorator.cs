using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.MongoDB.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.MongoDB;
using System.Text.Json;

namespace Shamdev.TOA.Web.Cache
{
    /// <summary>
    /// Добавляет работу с кешем 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ProcessingObjectLogDecorator<TEntity> : IProcessingObject<TEntity>
        where TEntity : DomainObject, new()
    {
        IProcessingObject<TEntity> _processingObject;
        ILog _log;
        public ProcessingObjectLogDecorator(IProcessingObject<TEntity> processingObject, ILog log)
        {
            _log = log;
            _processingObject = processingObject;
        }
        public IDefaultCRUDBLL<TEntity> GetCRUDHandler()
        {
            IDefaultCRUDBLL<TEntity> defaultCRUDBLL = _processingObject.GetCRUDHandler();
            if (_log != null)
                defaultCRUDBLL.DomainChangeEvent += new IDefaultCRUDBLL<TEntity>.DomainChangeHandler<TEntity>((executeTypeCRUD, item) =>
                {
                    string actionData = JsonSerializer.Serialize(item);
                    string executeType = executeTypeCRUD.Value.ToString();

                    LogItem logItem = new LogItem(actionData, executeType);

                    _log.AddLog(logItem);


                });
            return defaultCRUDBLL;
        }

        public IFetchData<TEntity> GetFetchDataHandler()
        {
            return _processingObject.GetFetchDataHandler();
        }
    }
}
