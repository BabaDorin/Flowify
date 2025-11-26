using Microsoft.Extensions.DependencyInjection;

namespace Flowify;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        
        var handler = _serviceProvider.GetService(handlerType);
        
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
        }

        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest>.Handle));
        
        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found for handler type {handlerType.Name}");
        }

        var task = (Task)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
        
        await task;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = _serviceProvider.GetService(handlerType);
        
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
        }

        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle));
        
        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found for handler type {handlerType.Name}");
        }

        var task = (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
        
        return await task;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(nameof(INotificationHandler<INotification>.Handle));
            
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, new object[] { notification, cancellationToken })!;
                tasks.Add(task);
            }
        }

        await Task.WhenAll(tasks);
    }
}
