
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
            if(!string.IsNullOrWhiteSpace(configuration["SERVER"]) &&
                !string.IsNullOrWhiteSpace(configuration["DATABASE_NAME"]) &&
                !string.IsNullOrWhiteSpace(configuration["USER_BD"]) &&
                !string.IsNullOrWhiteSpace(configuration["USER_BD_PASSWORD"])) 
               connection = $"Server={configuration["SERVER"]};Database={configuration["DATABASE_NAME"]};User ID={configuration["USER_BD"]};Password={configuration["USER_BD_PASSWORD"]}";
        
            services.AddDbContext<TContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IApplicationContext, TContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }



    }
}
