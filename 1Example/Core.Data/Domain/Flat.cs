using Shamdev.TOA.Core.Data;
using System.Text.Json.Serialization;

namespace Core.Data.Domain
{
    public class Flat : DomainObject
    {
        public int? Number { get; set; } // Номер дома

        public long? HouseId { get; set; }
        [JsonIgnore]
        public House House { get; set; }
    }
}
