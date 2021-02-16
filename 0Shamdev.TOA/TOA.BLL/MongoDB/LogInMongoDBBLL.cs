using Microsoft.Extensions.Configuration;
using Shamdev.ERP.Core.Data.Infrastructure.Interface;
using Shamdev.TOA.BLL.MongoDB.Interface;
using Shamdev.TOA.Core.Data.MongoDB;
using Shamdev.TOA.DAL.MongoDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.MongoDB
{
    public class LogInMongoDBBLL : ILog
    {
        MongoDBContext<LogItem> _mongoDBContext;
        public LogInMongoDBBLL(IGetEnvironment configuration)
        {
            string connectionString = configuration.GetMongoDBConnectionString;

            _mongoDBContext = new MongoDBContext<LogItem>(connectionString);
        }
        public async Task AddLog(LogItem log)
        {
            await _mongoDBContext.Create(log);
        }
    }
}
