using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Infrastructure;
using System.Threading.Tasks;

namespace Shamdev.TOA.DAL
{
    public interface IRepository<TEntity> where TEntity : DomainObject
    {
        //Получение данных
        Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery);
        Task<TEntity> GetByIdAsync(long id);

        //CRUD
        void Add(TEntity item);
        TEntity Create();
        TEntity Delete(long id);
    }
}
