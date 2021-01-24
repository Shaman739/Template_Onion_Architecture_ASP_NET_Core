using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using BLL.Common;
using Core.Data.Domain;
using Shamdev.TOA.BLL;
using BLL.Common.House;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery;
using Newtonsoft.Json.Converters;
using System;
using Newtonsoft.Json.Linq;
using DAL.Common;
using Shamdev.TOA.BLL.Interface;

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
            services.AddTransient<IProcessingObject<House>, ProcessingHouse>();
            services.AddTransient<IProcessingObject<Street>, ProcessingDomainObject<Street>>();

            //Добавление TOA зависимостей
            services.AddOnionArchitecture<RegisterApplicationContext>(Configuration);


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
            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

       

    }
}
