A simple and straightforward mediator and dispatching library for .NET 9+. Flowify helps decouple requests from handlers, promoting clean architecture and separation of concerns. It's a free, lightweight alternative to MediatR.

<img width="1279" height="694" alt="Flowify" src="https://github.com/user-attachments/assets/458807dd-20e0-45e5-adcf-0c355b7c9279" />


## Features

- **Commands** - With or without return values
- **Queries** - Retrieve data efficiently
- **Notifications** - Publish events to multiple handlers
- **Automatic Handler Registration** - Scan assemblies and register handlers automatically
- **MediatR-Compatible** - Same interfaces for easy migration

## Quick Example

```csharp
using Flowify;
using Microsoft.Extensions.DependencyInjection;

// Define a command
public record CreateUserCommand(string Name, string Email) : IRequest<int>;

// Create a handler
public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
{
    public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Your business logic here
        var userId = SaveUser(request.Name, request.Email);
        return Task.FromResult(userId);
    }
}

// Setup DI and use
var services = new ServiceCollection();
services.AddFlowify(typeof(Program).Assembly);
var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();

// Send command
var userId = await mediator.Send(new CreateUserCommand("John Doe", "john@example.com"));
```

## More Examples

See the [Examples](Examples/) folder for complete working examples including:
- Commands with/without responses
- Queries
- Notifications with multiple handlers

## License

MIT
