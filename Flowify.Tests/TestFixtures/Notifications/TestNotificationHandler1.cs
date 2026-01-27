using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Notifications;

public class TestNotificationHandler1 : INotificationHandler<TestNotification>
{
    public List<string> HandledMessages { get; } = new();

    public Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        HandledMessages.Add($"Handler1: {notification.Message}");
        return Task.CompletedTask;
    }
}
