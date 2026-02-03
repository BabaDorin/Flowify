using MediatR;

namespace Benchmarks.MediatR.Notifications;

public class MediatRPingNotificationHandler2 : INotificationHandler<MediatRPingNotification>
{
    public Task Handle(MediatRPingNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
