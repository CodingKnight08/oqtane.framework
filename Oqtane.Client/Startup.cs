#define ServerSideBlazor

using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Oqtane.Services;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Oqtane.Modules;

namespace Oqtane.Client
{
    public class Startup
    {
#if DEBUG || RELEASE
        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IComponentsApplicationBuilder app)
        {

        }
#endif
#if WASM
        public void ConfigureServices(IServiceCollection services)
        {
            // register singleton core services
            services.AddSingleton<IModuleDefinitionService, ModuleDefinitionService>();
            services.AddSingleton<ISkinService, SkinService>();

            // register scoped core services
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ISiteService, SiteService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IPageModuleService, PageModuleService>();
            services.AddScoped<IUserService, UserService>();

            // dynamically register module contexts and repository services
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] implementationtypes = assembly.GetTypes()
                    .Where(item => item.GetInterfaces().Contains(typeof(IService)))
                    .ToArray();
                foreach (Type implementationtype in implementationtypes)
                {
                    Type servicetype = Type.GetType(implementationtype.FullName.Replace(implementationtype.Name, "I" + implementationtype.Name));
                    if (servicetype != null)
                    {
                        services.AddScoped(servicetype, implementationtype); // traditional service interface
                    }
                    else
                    {
                        services.AddScoped(implementationtype, implementationtype); // no interface defined for service
                    }
                }
            }
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
#endif
    }
}
