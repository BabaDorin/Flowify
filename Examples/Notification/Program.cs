using Flowify;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Examples.Notification;

public record UserCreatedNotification(int UserId, string Name, string Email) : INotification;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[SendWelcomeEmailHandler] Sending welcome email to {notification.Email}");
        Console.WriteLine($"  Subject: Welcome, {notification.Name}!");
        Console.WriteLine($"  Body: Thank you for joining our platform.\n");
        
        return Task.CompletedTask;
    }
}

public class UpdateStatisticsDashboardHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[UpdateStatisticsDashboardHandler] Updating statistics dashboard");
        Console.WriteLine($"  New user count incremented");
        Console.WriteLine($"  User ID {notification.UserId} added to dashboard\n");
        
        return Task.CompletedTask;
    }
}

public class InvalidateCacheHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[InvalidateCacheHandler] Invalidating user cache");
        Console.WriteLine($"  Cache key 'users:list' invalidated");
        Console.WriteLine($"  Cache key 'users:count' invalidated\n");
        
        return Task.CompletedTask;
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(Program).Assembly);
        
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        Console.WriteLine("Publishing UserCreatedNotification...\n");
        
        var notification = new UserCreatedNotification(
            UserId: 42,
            Name: "Jane Doe",
            Email: "jane.doe@example.com"
        );
        
        await mediator.Publish(notification);
        
        Console.WriteLine("All notification handlers executed successfully!");
    }
}
