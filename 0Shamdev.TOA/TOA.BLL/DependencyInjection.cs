
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shamdev.ERP.Core.Data.Infrastructure.Interface;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;
using System;

namespace TOA.BLL
{
    public static class DependencyInjection
    {
        public static void AddBLL<TContext>(this IServiceCollection services, IConfiguration configuration, IGetEnvironment getEnvironment)
            where TContext: DbContext, IApplicationContext
        {

            services.AddDbContext<TContext>(options => options.UseSqlServer(getEnvironment.GetSqlConnectionString));
            services.AddTransient<IApplicationContext, TContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }



    }
}
