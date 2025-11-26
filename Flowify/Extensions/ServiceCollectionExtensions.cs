using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlowify(this IServiceCollection services, params Assembly[] assemblies)
    {
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
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType)
                {
                    var genericTypeDefinition = @interface.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(IRequestHandler<>) || 
                        genericTypeDefinition == typeof(IRequestHandler<,>))
                    {
                        services.AddTransient(@interface, type);
                    }
                    else if (genericTypeDefinition == typeof(INotificationHandler<>))
                    {
                        services.AddTransient(@interface, type);
                    }
                }
            }
        }
    }
}
