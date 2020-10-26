using DAL.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;

namespace BLL.Common
{
    public static class DependencyInjection
    {
        public static void AddBLL(this IServiceCollection services, IConfiguration configuration)
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<RegisterApplicationContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IApplicationContext, RegisterApplicationContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
