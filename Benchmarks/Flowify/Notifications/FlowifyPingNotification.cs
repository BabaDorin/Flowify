using Flowify.Contracts;

namespace Benchmarks.Flowify.Notifications;

public record FlowifyPingNotification(string Message) : INotification;
