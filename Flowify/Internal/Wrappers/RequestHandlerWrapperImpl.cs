using Flowify.Contracts;

namespace Flowify.Internal
{
    internal sealed class RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper
        where TRequest : IRequest
    {
        public override Task Handle(
            IRequest request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var typedRequest = (TRequest)request;
            var handler = (IRequestHandler<TRequest>?)serviceProvider.GetService(typeof(IRequestHandler<TRequest>));

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for request type {typeof(TRequest).Name}");
            }

            return handler.Handle(typedRequest, cancellationToken);
        }
    }
}
