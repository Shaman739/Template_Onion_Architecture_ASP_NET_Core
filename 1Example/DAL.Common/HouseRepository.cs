using Core.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Common
{
    public class HouseRepository : Repository<House>
    {
        public HouseRepository(DbContext contextDB) : base(contextDB)
        {
        }
        protected override IQueryable<House> DbSetWithInclude => base.DbSetWithInclude.Include(x=>x.Flats);
    }
}
