﻿using Shamdev.TOA.Core.Data;
using System.Collections.Generic;

namespace Core.Data.Domain
{
    public class House : DomainObject
    {
        public int? Number { get; set; } // Номер дома
        public List<Flat> Flats{ get; set; }
}
}
