using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Notifications;

public class TestNotificationHandler2 : INotificationHandler<TestNotification>
{
    public List<string> HandledMessages { get; } = new();

    public Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        HandledMessages.Add($"Handler2: {notification.Message}");
        return Task.CompletedTask;
    }
}
