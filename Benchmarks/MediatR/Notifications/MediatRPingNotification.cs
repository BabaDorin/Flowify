using MediatR;

namespace Benchmarks.MediatR.Notifications;

public record MediatRPingNotification(string Message) : INotification;
