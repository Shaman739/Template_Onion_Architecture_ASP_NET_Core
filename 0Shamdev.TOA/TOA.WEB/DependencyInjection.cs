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
using Shamdev.TOA.WEB.Cache.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Web.Cache;
using Shamdev.TOA.DAL.Interface;
using Shamdev.TOA.WEB.Cache;
using System;
using Shamdev.TOA.Web.Infrastructure;
using Shamdev.ERP.Core.Data.Infrastructure.Interface;
using Shamdev.TOA.Web;
using Microsoft.AspNetCore.Http;

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
        public static void AddOnionArchitecture<TContext>(this IServiceCollection services, IConfiguration configuration, IGetEnvironment environment)
             where TContext : DbContext, IApplicationContext
        {
            services.AddBLL<TContext>(configuration, environment);

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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserContext, UserContext>();
        }

            /// <summary>
            /// Добавляет кеш через AddSingleton для типа и декорирует стандартный БЛЛ IProcessingObject
            /// </summary>
            /// <typeparam name="TEntity"></typeparam>
            /// <typeparam name="TCacheClass"></typeparam>
            /// <param name="services"></param>
            /// <param name="configuration"></param>
            public static void AddCacheInBLL<TEntity, TCacheClass>(this IServiceCollection services, IConfiguration configuration)
            where TEntity: DomainObject,new()
            where TCacheClass : class, ICache<TEntity>
        {
            services.AddSingleton<ICache<TEntity>, TCacheClass>();
            services.Decorate<IProcessingObject<TEntity>, ProcessingObjectCacheDecorator<TEntity>>();

        }

       

        
    }
}
