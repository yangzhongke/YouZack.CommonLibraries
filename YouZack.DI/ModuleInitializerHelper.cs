using AutoMapper;
using Infrastructures.DI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ModuleInitializerHelper
    {
        public static void RegisterAllServices(this IServiceCollection services, Action<DbContextOptionsBuilder> dbCtxOptBuilder, params Assembly[] assemblies)
        {
            services.RegisterDbContexts(dbCtxOptBuilder, assemblies);
            services.RegisterRepositories(assemblies);
            services.RegisterServices(assemblies);
            services.RunModuleInitializers(assemblies);
            services.AddAutoMapper(assemblies);//AddAutoMapper can only be called once,because it will overwrite previous setup.
        }

        /// <summary>
        /// 每个项目中都可以自己写一些实现了IModuleInitializer接口的类，在其中注册自己需要的服务，这样避免什么都要到入口项目中注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        public static void RunModuleInitializers(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var implType in assemblies.SelectMany(asm => asm.GetTypes())
                .Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t)))
            {
                services.AddScoped(typeof(IModuleInitializer), implType);
            }
            using (var sp = services.BuildServiceProvider())
            {
                var moduleInitializers = sp.GetService<IEnumerable<IModuleInitializer>>();
                foreach (var initializer in moduleInitializers)
                {
                    initializer.Initialize(services);
                }
            }
        }


        public static void RegisterDbContexts(this IServiceCollection services, Action<DbContextOptionsBuilder> builder, params Assembly[] assemblies)
        {
            var methodAddDbContextPool = typeof(EntityFrameworkServiceCollectionExtensions).GetMethod(nameof(EntityFrameworkServiceCollectionExtensions.AddDbContextPool), 1,
                new Type[] { typeof(IServiceCollection), typeof(Action<DbContextOptionsBuilder>), typeof(int) });
            object defaultValueOfPoolSize = methodAddDbContextPool.GetParameters()[2].DefaultValue;//get default value of parameter 'int poolSize'

            foreach (var asmToLoad in assemblies)
            {
                //Register DbContext
                //GetTypes() include public/protected
                //GetExportedTypes only include public
                //so that XXDbContext in Agrregation can be internal to keep insulated
                foreach (var dbCtxType in asmToLoad.GetTypes()
                    .Where(t => !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t)))
                {
                    //similar to serviceCollection.AddDbContextPool<ECDictDbContext>(opt=>new DbContextOptionsBuilder(dbCtxOpt));
                    var methodGenericAddDbContextPool = methodAddDbContextPool.MakeGenericMethod(dbCtxType);
                    methodGenericAddDbContextPool.Invoke(null, new object[] { services, builder, defaultValueOfPoolSize });
                }
            }
        }

        public static void RegisterRepositories(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var asmToLoad in assemblies)
            {
                //IRepository
                foreach (var repositoryImplType in asmToLoad.GetTypes()
                .Where(t => !t.IsAbstract && typeof(IRepository).IsAssignableFrom(t)))
                {
                    //only registr direct parent-interfaces
                    foreach (var intfType in repositoryImplType.GetInterfaces())
                    {
                        services.AddScoped(intfType, repositoryImplType);
                    }
                }
            }
        }

        public static void RegisterServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var asmToLoad in assemblies)
            {
                //IService
                foreach (var serviceImplType in asmToLoad.GetTypes()
                .Where(t => !t.IsAbstract && typeof(IService).IsAssignableFrom(t)))
                {
                    //only registr direct parent-interfaces
                    foreach (var intfType in serviceImplType.GetInterfaces())
                    {
                        services.AddScoped(intfType, serviceImplType);
                    }
                }
            }
        }
    }
}
