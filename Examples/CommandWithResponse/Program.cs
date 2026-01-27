using Flowify.Contracts;
using Flowify.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Examples.CommandWithResponse;

public record CreateUserCommand(string Name, string Email) : IRequest<int>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
{
    public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating user: {request.Name} with email: {request.Email}");
        
        var userId = Random.Shared.Next(1, 1000);
        
        Console.WriteLine($"User created with ID: {userId}");
        
        return Task.FromResult(userId);
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
        
        var command = new CreateUserCommand("John Doe", "john.doe@example.com");
        var userId = await mediator.Send(command);
        
        Console.WriteLine($"Command returned user ID: {userId}");
    }
}
