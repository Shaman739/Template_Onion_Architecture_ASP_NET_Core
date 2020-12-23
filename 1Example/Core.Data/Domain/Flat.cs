using Shamdev.TOA.Core.Data;
using System.Text.Json.Serialization;

namespace Core.Data.Domain
{
    public class Flat : DomainObject
    {
        /// <summary>
        /// Номер квартиры
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Количество проживающих в квартире
        /// </summary>
        public int? NumberOfPersonLiving { get; set; }
        /// <summary>
        /// Площать квартиры
        /// </summary>
        public decimal? Area { get; set; }
        public long? HouseId { get; set; }
        [JsonIgnore]
        public House House { get; set; }
    }
}
