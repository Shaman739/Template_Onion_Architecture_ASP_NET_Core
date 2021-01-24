

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.Common;
using TOA.BLL;
using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.DAL;
using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Shamdev.TOA.BLL.Service;
using Shamdev.TOA.BLL.Service.Interface;
using Shamdev.TOA.BLL.Interface;

namespace BLL.Common
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавляет зависимости по TOA Onion Architecture 
        /// </summary>
        /// <typeparam name="TContext">Контекст БД</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddOnionArchitecture<TContext>(this IServiceCollection services, IConfiguration configuration)
             where TContext : DbContext, IApplicationContext
        {
            services.AddBLL<TContext>(configuration);

            services.AddTransient<IProcessingObject<User>, ProcessingDomainObject<User>>();
            services.AddTransient<IAccountService,AccountService> ();
            
            services
               .AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.IgnoreNullValues = true;
               })
               .AddNewtonsoftJson(o =>
               {

                   o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                   o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                   o.SerializerSettings.Converters.Add(new StringEnumConverter());
                   o.SerializerSettings.Converters.Add(new JsonInt32Converter());

               });

            // установка конфигурации подключения
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => //CookieAuthenticationOptions
                    {
                        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Login");
                    });
        }
    }
}
