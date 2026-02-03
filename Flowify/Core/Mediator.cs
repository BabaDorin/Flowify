using System.Collections.Concurrent;
using Flowify.Contracts;
using Flowify.Internal;

namespace Flowify.Core
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper> RequestHandlerWrapperCache = new();
        private static readonly ConcurrentDictionary<Type, object> RequestWithResponseHandlerWrapperCache = new();
        private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationHandlerWrapperCache = new();

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var wrapper = RequestHandlerWrapperCache.GetOrAdd(
                requestType,
                static type => CreateRequestHandlerWrapper(type));

            return wrapper.Handle(request, _serviceProvider, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var wrapper = (RequestHandlerWrapper<TResponse>)RequestWithResponseHandlerWrapperCache.GetOrAdd(
                requestType,
                type => CreateRequestWithResponseHandlerWrapper(type, typeof(TResponse)));

            return wrapper.Handle(request, _serviceProvider, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var notificationType = notification.GetType();
            var wrapper = NotificationHandlerWrapperCache.GetOrAdd(
                notificationType,
                static type => CreateNotificationHandlerWrapper(type));

            return wrapper.Handle(notification, _serviceProvider, cancellationToken);
        }

        private static RequestHandlerWrapper CreateRequestHandlerWrapper(Type requestType)
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType);
            return (RequestHandlerWrapper)Activator.CreateInstance(wrapperType)!;
        }

        private static object CreateRequestWithResponseHandlerWrapper(Type requestType, Type responseType)
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType);
            return Activator.CreateInstance(wrapperType)!;
        }

        private static NotificationHandlerWrapper CreateNotificationHandlerWrapper(Type notificationType)
        {
            var wrapperType = typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(notificationType);
            return (NotificationHandlerWrapper)Activator.CreateInstance(wrapperType)!;
        }
    }
}
