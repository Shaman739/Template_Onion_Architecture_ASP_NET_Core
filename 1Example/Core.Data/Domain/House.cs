using Shamdev.ERP.Core.Data.Interface;
using Shamdev.TOA.Core.Data;
using System.Collections.Generic;

namespace Core.Data.Domain
{
    public class House : DomainObject, IDomainObjectIdentity
    {
        /// <summary>
        /// Номер дома
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Количество подъездов
        /// </summary>
        public int? CountOfEntrance { get; set; }
        /// <summary>
        /// Количество зтажей
        /// </summary>
        public int? CountOfFloor { get; set; }

        public List<Flat> Flats{ get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long? UserId { get; set; }
    }
}
