using Shamdev.TOA.Core.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.DAL.MongoDB
{
    /// <summary>
    /// Работа с MongoBD
    /// </summary>
    public class MongoDBContext<TEntity>
        where TEntity: IdentityMongoDBDocument
    {
        IMongoDatabase database; // база данных

        private MongoDBContext()
        {

        }
        public MongoDBContext(string connectionString) : this()
        {

            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            database = client.GetDatabase(connection.DatabaseName);
        }
        /// <summary>
        /// Коллекция документов
        /// </summary>
        public IMongoCollection<TEntity> CollectionInBD
        {
            get { return database.GetCollection<TEntity>(typeof(TEntity).Name.ToLower()); }
        }
        // получаем один документ по id
        public async Task<TEntity> Get(string id)
        {
            return await CollectionInBD.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task Create(TEntity c)
        {
            await CollectionInBD.InsertOneAsync(c);
        }
        // обновление документа
        public async Task Update(TEntity c)
        {
            await CollectionInBD.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(c.Id)), c);
        }
        // удаление документа
        public async Task Remove(string id)
        {
            await CollectionInBD.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

    }
}
