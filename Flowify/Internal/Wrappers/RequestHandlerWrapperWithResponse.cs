using Flowify.Contracts;

namespace Flowify.Internal
{
    internal abstract class RequestHandlerWrapper<TResponse>
    {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}
