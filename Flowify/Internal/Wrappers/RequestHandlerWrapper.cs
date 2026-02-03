using Flowify.Contracts;

namespace Flowify.Internal
{
    internal abstract class RequestHandlerWrapper
    {
        public abstract Task Handle(
            IRequest request,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}
