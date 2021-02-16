using Microsoft.Extensions.Configuration;
using Shamdev.ERP.Core.Data.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.TOA.Web.Infrastructure
{
    public class GetEnvironment : IGetEnvironment
    {
        private IConfiguration _configuration;

        private GetEnvironment()
        {

        }
        public GetEnvironment(IConfiguration configuration)
        {
            _configuration = configuration;
            if (_configuration == null) throw new ArgumentNullException();
        }
        public string GetMongoDBConnectionString
        {
            get
            {
                string connectionstring = "";
                if (!string.IsNullOrWhiteSpace(_configuration["MONGODB_IP"]) &&
                    !string.IsNullOrWhiteSpace(_configuration["MONGODB_PORT"]) &&
                    !string.IsNullOrWhiteSpace(_configuration["MONGODB_DB_NAME"]))
                    connectionstring = $"mongodb://{_configuration["MONGODB_IP"]}:{_configuration["MONGODB_PORT"]}/{_configuration["MONGODB_DB_NAME"]}";

                return connectionstring;
            }
        }
        public string GetSqlConnectionString
        {
            get
            {
                string connectionstring = "";
                if (!string.IsNullOrWhiteSpace(_configuration["SERVER"]) &&
                    !string.IsNullOrWhiteSpace(_configuration["DATABASE_NAME"]) &&
                    !string.IsNullOrWhiteSpace(_configuration["USER_BD"]) &&
                    !string.IsNullOrWhiteSpace(_configuration["USER_BD_PASSWORD"]))
                    connectionstring = $"Server={_configuration["SERVER"]};Database={_configuration["DATABASE_NAME"]};User ID={_configuration["USER_BD"]};Password={_configuration["USER_BD_PASSWORD"]}";
                return connectionstring;
            }
        }
    }
}
