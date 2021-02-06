using Microsoft.Extensions.DependencyInjection;
using SuperCode.Services;

namespace SuperCode
{
    public static class Extension
    {
        /// <summary>
        /// 添加 SuperCode 相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSuperCodeServices(this IServiceCollection services)
        {
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IOnlineTemplateService, OnlineTemplateService>();
            return services;
        }

    }
}
