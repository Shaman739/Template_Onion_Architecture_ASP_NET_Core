using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BLL.Common
{
    public static class DependencyInjection
    {
        //public static void AddOnionArchitecture(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddControllers()
        //    .AddJsonOptions(options =>
        //    {
        //        options.JsonSerializerOptions.IgnoreNullValues = true;
        //    })
        //    .AddNewtonsoftJson(o =>
        //    {
        //        o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        //        o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //    });
        //}
    }
}
