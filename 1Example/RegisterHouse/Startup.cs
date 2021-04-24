using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BLL.Common;
using Core.Data.Domain;
using Shamdev.TOA.BLL;
using BLL.Common.House;

using DAL.Common;
using Shamdev.TOA.BLL.Interface;

using Shamdev.TOA.WEB.Cache;
using Shamdev.TOA.Web.Cache;
using Shamdev.TOA.BLL.MongoDB.Interface;
using Shamdev.TOA.BLL.MongoDB;
using Shamdev.TOA.Web.Infrastructure;
using Shamdev.ERP.Core.Data.Infrastructure.Interface;
using Shamdev.TOA.BLL.Decorators;
using Shamdev.TOA.DAL.Infrastructure;

namespace RegisterHouse
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddSingleton<IGetEnvironment, GetEnvironment>();
          
            services.AddTransient<ILog, LogInMongoDBBLL>();

            //House
            services.AddTransient<IDefaultCRUDBLL<House>, HouseBLL>();
            services.AddTransient<IFetchData<House>, FetchDomainData<House>>();
            services.AddCacheInBLL<House, MemoryCacheRepository<House>>(Configuration);
            services.Decorate<IDefaultCRUDBLL<House>, LogerCRUDBLLDecoratorDecorator<House>>();
            services.Decorate<IFetchData<House>, UserDataSecurityFetchDomainData<House>>();
          

            //Street
            services.AddTransient<IDefaultCRUDBLL<Street>, DefaultCRUDBLL<Street>>();
            services.AddTransient<IFetchData<Street>, FetchDomainData<Street>>();
            services.AddCacheInBLL<Street, MemoryCacheRepository<Street>>(Configuration);
            //Добавление TOA зависимостей

            services.AddOnionArchitecture<RegisterApplicationContext>(Configuration, services.BuildServiceProvider().GetService<IGetEnvironment>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          
            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors(builder =>
           builder.WithOrigins("http://localhost:4200")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials());

            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

       

    }
}
