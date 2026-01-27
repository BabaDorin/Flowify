using System.Collections.Concurrent;
using System.Reflection;
using Flowify.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Core
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, Type> RequestHandlerTypeCache = new();
        private static readonly ConcurrentDictionary<Type, Type> RequestWithResponseHandlerTypeCache = new();
        private static readonly ConcurrentDictionary<Type, Type> NotificationHandlerTypeCache = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethodCache = new();

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
            var handlerType = RequestHandlerTypeCache.GetOrAdd(
                requestType,
                t => typeof(IRequestHandler<>).MakeGenericType(t));

            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
            }

            var handleMethod = HandleMethodCache.GetOrAdd(
                handlerType,
                t => t.GetMethod(nameof(IRequestHandler<IRequest>.Handle))!);

            try
            {
                var task = (Task)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
                await task.ConfigureAwait(false);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            
            var handlerType = RequestWithResponseHandlerTypeCache.GetOrAdd(
                requestType,
                t => typeof(IRequestHandler<,>).MakeGenericType(t, typeof(TResponse)));

            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
            }

            var handleMethod = HandleMethodCache.GetOrAdd(
                handlerType,
                t => t.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!);

            try
            {
                var task = (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
                return await task.ConfigureAwait(false);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var notificationType = notification.GetType();
            var handlerType = NotificationHandlerTypeCache.GetOrAdd(
                notificationType,
                t => typeof(INotificationHandler<>).MakeGenericType(t));

            var handlers = _serviceProvider.GetServices(handlerType);

            var handleMethod = HandleMethodCache.GetOrAdd(
                handlerType,
                t => t.GetMethod(nameof(INotificationHandler<INotification>.Handle))!);

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                if (handler == null) continue;

                async Task ExecuteHandler()
                {
                    try
                    {
                        var task = (Task)handleMethod.Invoke(handler, new object[] { notification, cancellationToken })!;
                        await task.ConfigureAwait(false);
                    }
                    catch (TargetInvocationException ex) when (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                }

                tasks.Add(ExecuteHandler());
            }

            if (tasks.Count == 0) return;

            var allTasks = Task.WhenAll(tasks);

            try
            {
                await allTasks.ConfigureAwait(false);
            }
            catch
            {
                var exceptions = allTasks.Exception?.InnerExceptions.ToList() ?? new List<Exception>();

                if (exceptions.Count == 1)
                {
                    throw exceptions[0];
                }

                if (exceptions.Count > 1)
                {
                    throw new AggregateException("One or more notification handlers failed.", exceptions);
                }

                throw;
            }
        }
    }
}
