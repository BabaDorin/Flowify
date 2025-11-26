using Flowify;
using Microsoft.Extensions.DependencyInjection;

namespace Flowify.Examples.Query;

public record UserModel(int Id, string Name, string Email, DateTime CreatedAt);

public record GetUserByIdQuery(int Id) : IRequest<UserModel>;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserModel>
{
    private static readonly List<UserModel> _users = new()
    {
        new UserModel(1, "Alice Johnson", "alice@example.com", DateTime.Now.AddDays(-30)),
        new UserModel(2, "Bob Smith", "bob@example.com", DateTime.Now.AddDays(-15)),
        new UserModel(3, "Charlie Brown", "charlie@example.com", DateTime.Now.AddDays(-5))
    };

    public Task<UserModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Querying user with ID: {request.Id}");
        
        var user = _users.FirstOrDefault(u => u.Id == request.Id);
        
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.Id} not found");
        }
        
        Console.WriteLine($"Found user: {user.Name}");
        
        return Task.FromResult(user);
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
        
        var query = new GetUserByIdQuery(2);
        var user = await mediator.Send(query);
        
        Console.WriteLine($"\nQuery Result:");
        Console.WriteLine($"  ID: {user.Id}");
        Console.WriteLine($"  Name: {user.Name}");
        Console.WriteLine($"  Email: {user.Email}");
        Console.WriteLine($"  Created: {user.CreatedAt:yyyy-MM-dd}");
    }
}
