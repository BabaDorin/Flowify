using System.Reflection;
using Flowify.Contracts;
using Flowify.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFlowify(this IServiceCollection services, params Assembly[]? assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            services.AddTransient<IMediator, Mediator>();

            foreach (var assembly in assemblies)
            {
                RegisterHandlers(services, assembly);
            }

            return services;
        }

        private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            var concreteTypes = types
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var type in concreteTypes)
            {
                RegisterType(services, type);
            }
        }

        private static void RegisterType(IServiceCollection services, Type type)
        {
            var interfaces = type.GetInterfaces();

            foreach (var entry in interfaces)
            {
                if (!entry.IsGenericType) continue;

                RegisterConcreteTypeForInterface(services, type, entry);
            }
        }

        private static void RegisterConcreteTypeForInterface(IServiceCollection services, Type type, Type entry)
        {
            var genericTypeDefinition = entry.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(IRequestHandler<>) || genericTypeDefinition == typeof(IRequestHandler<,>))
            {
                services.AddTransient(entry, type);
            }
            else if (genericTypeDefinition == typeof(INotificationHandler<>))
            {
                services.AddTransient(entry, type);
            }
        }
    }
}
