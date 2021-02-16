using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.ERP.Core.Data.Infrastructure.Interface
{
    public interface IGetEnvironment
    {
        string GetMongoDBConnectionString { get;}
        string GetSqlConnectionString { get; }
    }
}
