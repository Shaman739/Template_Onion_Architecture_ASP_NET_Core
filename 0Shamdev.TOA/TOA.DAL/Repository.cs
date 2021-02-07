using Shamdev.TOA.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Shamdev.TOA.DAL.Infrastructure;

namespace Shamdev.TOA.DAL
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : DomainObject, new()
    {
        private DbContext _contextDB;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(DbContext contextDB)
        {
            _contextDB = contextDB;
            _dbSet = _contextDB.Set<TEntity>();
        }
        /// <summary>
        /// Для получения записей со связанными объектами 
        /// </summary>
        protected virtual IQueryable<TEntity> DbSetWithInclude => _dbSet; 
        

        /// <summary>
        /// Метод, который получает данные из БД
        /// </summary>
        /// <param name="paramQuery">Параметры запроса: номер страницы, размер страницы. По дефолту размер страницы 40 записей</param>
        /// <returns></returns>
        public async Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery)
        {
            if (paramQuery == null) paramQuery = new FetchDataParameters();
            paramQuery.CheckAndResetParam();

            ResultFetchData<TEntity> result = new ResultFetchData<TEntity>();

            result.TotalCountRows = await DbSetWithInclude.AsNoTracking().CountAsync();

            int startRow = (paramQuery.PageNumber - 1) * paramQuery.CountOnPage;
            IQueryable<TEntity> query = DbSetWithInclude.Skip(startRow).Take((int)paramQuery.CountOnPage);

            if (paramQuery.IsOnlyShowData)
                query = query.AsNoTracking();

            result.Items = await query.ToListAsync();
            result.PageNumber = paramQuery.PageNumber;
            return result;
        }

        /// <summary>
        /// Добавляет объект в контекст
        /// </summary>
        /// <param name="item"></param>
        public void Add(TEntity item)
        {
            _dbSet.Add(item);
        }
        /// <summary>
        /// Создает объект и добавляет его в контекст
        /// </summary>
        /// <returns>Новый объект, который в контексте </returns>
        public TEntity Create()
        {
            TEntity item = new TEntity();
            _dbSet.Add(item);
            return item;
        }
        public async Task<TEntity> GetByIdAsync(long id)
        {
            return await DbSetWithInclude.SingleOrDefaultAsync(x=>x.Id == id);
        }

        public TEntity Delete(long id)
        {
            TEntity item = _dbSet.Find(id);
            if (item == null)
                throw new ArgumentException("Записи для удаления не существует.");
            _dbSet.Remove(item);
            return item;

        }
    }
}
