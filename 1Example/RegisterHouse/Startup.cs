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
            services.AddTransient<IDefaultCRUDBLL<House>, HouseBLL>();
            //Добавляем Onion Architecture
            services.AddBLL(Configuration);
           // services.AddOnionArchitecture(Configuration);
            services.AddControllers()
           .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.IgnoreNullValues = true;
           })
           .AddNewtonsoftJson(o =>
           {
               o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
               o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
               o.SerializerSettings.Converters.Add(new StringEnumConverter());
           });

            //.ConfigureApiBehaviorOptions(options =>
            //{
            //    options.InvalidModelStateResponseFactory = context =>
            //    {
            //        var result = new BadRequestObjectResult(context.ModelState);

            //        // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
            //        result.ContentTypes.Add(MediaTypeNames.Application.Json);
            //        result.ContentTypes.Add(MediaTypeNames.Application.Xml);
            //       // return new FailResultQuery() { Message = result.ToString() };
            //        return result;
            //    };
            //});

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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
