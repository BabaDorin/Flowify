using Flowify.Contracts;

namespace Flowify.Internal
{
    internal abstract class NotificationHandlerWrapper
    {
        public abstract Task Handle(
            INotification notification,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }
}
