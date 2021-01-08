
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;
using System;

namespace TOA.BLL
{
    public static class DependencyInjection
    {
        public static void AddBLL<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext: DbContext, IApplicationContext
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IApplicationContext, TContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
