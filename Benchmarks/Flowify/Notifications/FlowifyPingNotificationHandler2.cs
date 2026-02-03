using Flowify.Contracts;

namespace Benchmarks.Flowify.Notifications;

public class FlowifyPingNotificationHandler2 : INotificationHandler<FlowifyPingNotification>
{
    public Task Handle(FlowifyPingNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
