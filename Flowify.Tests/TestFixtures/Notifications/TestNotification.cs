using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Notifications;

public record TestNotification(string Message) : INotification;
