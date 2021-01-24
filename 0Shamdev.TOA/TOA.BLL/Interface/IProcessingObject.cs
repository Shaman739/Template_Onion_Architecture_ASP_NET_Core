using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data;

namespace Shamdev.TOA.BLL.Interface
{
    public interface IProcessingObject<TEntity> where TEntity : DomainObject, new()
    {
        IDefaultCRUDBLL<TEntity> GetCRUDHandler();
        IFetchData<TEntity> GetFetchDataHandler();
    }
}