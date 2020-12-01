using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Utils
{
    //TODO: Добавить тест
    public class SynchronizeChildrenObject<TEntity>
        where TEntity : DomainObject, new()
    {
        List<TEntity> _listChildren;
        IDefaultCRUDBLL<TEntity> _bllChild;
        private SynchronizeChildrenObject() { }
        public SynchronizeChildrenObject(List<TEntity> listChildren, IDefaultCRUDBLL<TEntity> bllChild)
        {
            _listChildren = listChildren;
            if (_listChildren == null) throw new Exception("Отсутствует список дочерних элементов.");

            _bllChild = bllChild;
            if (_bllChild == null) throw new Exception("Отсутствует БЛЛ для дочерних элементов");

            _bllChild.IsOnlyAddInContext = true;
        }

        /// <summary>
        /// Добавление элементов
        /// </summary>
        /// <param name="newItems">Лист элементов</param>
        public void Synchonize(List<TEntity> newItems)
        {
            if (newItems == null) newItems = new List<TEntity>();
            AddOrUpdateItems(newItems);
            DeleteItems(newItems);
            //_listChildren.Clear();
            //_listChildren.AddRange(newItems);
        }

        /// <summary>
        /// Добавление или изменение записей 
        /// </summary>
        /// <param name="newItems">Список новых записей</param>
        private void AddOrUpdateItems(List<TEntity> newItems)
        {

            foreach (TEntity item in newItems)
            {
                ExecuteTypeConstCRUD executeTypeConstCRUD;
                BaseResultType<TEntity> itemFromDB = _bllChild.GetByIdAsync(item.Id).Result;
                if (!itemFromDB.IsSuccess)
                    executeTypeConstCRUD = ExecuteTypeConstCRUD.ADD;
                else
                    executeTypeConstCRUD = ExecuteTypeConstCRUD.EDIT;

                 CRUDOperation(item, executeTypeConstCRUD);
            }
        }

        /// <summary>
        /// Удаление записие, которых нет в новом списке
        /// </summary>
        /// <param name="newItems">Список новых записей</param>
        private void DeleteItems(List<TEntity> newItems)
        {
            //Выбирает записи, которых нет в новом списке, но есть в старом
            List<TEntity> listForDelete = _listChildren.Where(x => !newItems.Any(y => y.Id == x.Id)).ToList();
            foreach(TEntity item in listForDelete)
            {
                CRUDOperation(item, ExecuteTypeConstCRUD.DELETE);
                _listChildren.Remove(item);

            };
        }
        /// <summary>
        /// Выполенение CRUD операции по типу операции
        /// </summary>
        /// <param name="item"></param>
        /// <param name="executeTypeConstCRUD">Тип операции</param>
        private void CRUDOperation(TEntity item, ExecuteTypeConstCRUD executeTypeConstCRUD)
        {
            DefaultParamOfCRUDOperation<TEntity> param = new DefaultParamOfCRUDOperation<TEntity>();
            param.Item = item;
            BaseResultType<SaveResultType<TEntity>> resultSaveFlat = _bllChild.SaveItemAsync(executeTypeConstCRUD, param).Result;
            if (resultSaveFlat.IsSuccess)
            {
                if (item.Id != 0)
                {
                    TEntity deletedItem = _listChildren.Find(x => x.Id == item.Id);
                    if (deletedItem != null)
                        _listChildren.Remove(deletedItem);
                }
                _listChildren.Add(resultSaveFlat.Data.Item);
            }
            else
                throw new Exception(resultSaveFlat.Message);
        }
    }
}
