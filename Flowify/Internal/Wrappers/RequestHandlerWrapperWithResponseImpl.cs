using Flowify.Contracts;

namespace Flowify.Internal
{
    internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var typedRequest = (TRequest)request;
            var handler = (IRequestHandler<TRequest, TResponse>?)serviceProvider.GetService(
                typeof(IRequestHandler<TRequest, TResponse>));

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for request type {typeof(TRequest).Name}");
            }

            return handler.Handle(typedRequest, cancellationToken);
        }
    }
}
