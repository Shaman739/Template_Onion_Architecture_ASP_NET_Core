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

            services.AddTransient<IProcessingObject<House>, ProcessingHouse>();
            services.AddTransient<IProcessingObject<Street>, ProcessingDomainObject<Street>>();

            //��������� ��� 
            services.AddCacheInBLL<Street, MemoryCacheRepository<Street>>(Configuration);
            services.AddCacheInBLL<House, MemoryCacheRepository<House>>(Configuration);

            services.Decorate<IProcessingObject<House>, ProcessingObjectLogDecorator<House>>();

            //���������� TOA ������������
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
            app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader()
                            .AllowAnyMethod());
           // app.UseAuthorization();
            app.UseAuthentication();    // ��������������
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

       

    }
}
