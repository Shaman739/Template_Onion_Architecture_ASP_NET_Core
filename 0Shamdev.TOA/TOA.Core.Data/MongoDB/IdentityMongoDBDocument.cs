using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shamdev.TOA.Core.Data.MongoDB
{
    public abstract class IdentityMongoDBDocument
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}