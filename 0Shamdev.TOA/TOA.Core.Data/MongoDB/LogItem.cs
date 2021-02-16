using Shamdev.TOA.Core.Data.MongoDB;
using MongoDB.Bson;

using System;


namespace Shamdev.TOA.Core.Data.MongoDB
{
    /// <summary>
    /// Запись лога
    /// </summary>
    public class LogItem : IdentityMongoDBDocument
    {
        private LogItem()
        {
            ActionTime = DateTime.Now;
        }
        public LogItem(string actionData, string executeTypeCRUD) : this()
        {
            ActionTime = DateTime.Now;
            this.actionData = actionData;
            this.executeTypeCRUD = executeTypeCRUD;
        }
        /// <summary>
        /// Данные для опеарции
        /// </summary>
        public string actionData { get; private set; }
        /// <summary>
        /// Тип действия
        /// </summary>
        public string executeTypeCRUD { get; private set; }
        /// <summary>
        /// Время действия
        /// </summary>
        public BsonDateTime ActionTime { get; private set; }
    }
}
