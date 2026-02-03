using Flowify.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Flowify.Internal
{
    internal sealed class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
        where TNotification : INotification
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task Handle(
            INotification notification,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();
            return HandleCore((TNotification)notification, handlers, cancellationToken);
        }

        private static Task HandleCore(
            TNotification notification,
            IEnumerable<INotificationHandler<TNotification>> handlers,
            CancellationToken cancellationToken)
        {
            Task? pending1 = null;
            Task? pending2 = null;
            List<Task>? pendingList = null;

            foreach (var handler in handlers)
            {
                var task = handler.Handle(notification, cancellationToken);

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    continue;
                }

                if (pending1 == null)
                {
                    pending1 = task;
                }
                else if (pending2 == null)
                {
                    pending2 = task;
                }
                else
                {
                    pendingList ??= new List<Task>(4) { pending1, pending2 };
                    pendingList.Add(task);
                }
            }

            if (pending1 == null)
            {
                return Task.CompletedTask;
            }

            if (pending2 == null)
            {
                return pending1;
            }

            if (pendingList != null)
            {
                return Task.WhenAll(pendingList);
            }

            return Task.WhenAll(pending1, pending2);
        }
    }
}
