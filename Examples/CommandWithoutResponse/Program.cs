using Flowify.Contracts;
using Flowify.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Examples.CommandWithoutResponse;

public record SendEmailCommand(string To, string Subject, string Body) : IRequest;

public class SendEmailHandler : IRequestHandler<SendEmailCommand>
{
    public Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending email to: {request.To}");
        Console.WriteLine($"Subject: {request.Subject}");
        Console.WriteLine($"Body: {request.Body}");
        Console.WriteLine("Email sent successfully!");
        
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
        
        var command = new SendEmailCommand(
            "recipient@example.com",
            "Welcome!",
            "Thank you for joining our platform."
        );
        
        await mediator.Send(command);
        
        Console.WriteLine("Command executed without return value.");
    }
}
