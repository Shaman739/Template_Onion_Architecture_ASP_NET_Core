using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using BLL.Common;
using Core.Data.Domain;
using Shamdev.TOA.BLL;

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
            services.AddTransient<IDefaultCRUDBLL<House>, DefaultCRUDBLL<House>>();
            //Добавляем Onion Architecture
            services.AddBLL(Configuration);

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

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
