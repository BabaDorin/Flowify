# Flowify Examples

This folder contains working examples demonstrating different usage patterns of the Flowify library.

## Examples

### 1. CommandWithResponse
Demonstrates a command that returns a value.

**Command**: `CreateUserCommand` - Creates a user and returns the user ID
**Handler**: `CreateUserHandler` - Processes the command and returns an integer

Run:
```bash
cd CommandWithResponse
dotnet run
```

### 2. CommandWithoutResponse
Demonstrates a command that doesn't return a value (void).

**Command**: `SendEmailCommand` - Sends an email without returning anything
**Handler**: `SendEmailHandler` - Processes the command with no return value

Run:
```bash
cd CommandWithoutResponse
dotnet run
```

### 3. Query
Demonstrates a query that retrieves data.

**Query**: `GetUserByIdQuery` - Retrieves a user by ID
**Handler**: `GetUserByIdHandler` - Fetches and returns user data
**Model**: `UserModel` - The response object

Run:
```bash
cd Query
dotnet run
```

### 4. Notification
Demonstrates notifications with multiple handlers processing the same event.

**Notification**: `UserCreatedNotification` - Event raised when a user is created
**Handlers**:
- `SendWelcomeEmailHandler` - Sends a welcome email
- `UpdateStatisticsDashboardHandler` - Updates statistics
- `InvalidateCacheHandler` - Invalidates relevant caches

Run:
```bash
cd Notification
dotnet run
```

## Key Concepts

### Commands with Response
Use `IRequest<TResponse>` when your command needs to return a value:
```csharp
public record CreateUserCommand(string Name) : IRequest<int>;
public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
{
    public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Return the user ID
        return Task.FromResult(userId);
    }
}
```

### Commands without Response
Use `IRequest` when your command doesn't need to return anything:
```csharp
public record SendEmailCommand(string To) : IRequest;
public class SendEmailHandler : IRequestHandler<SendEmailCommand>
{
    public Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        // Just perform the action
        return Task.CompletedTask;
    }
}
```

### Queries
Queries are semantically the same as commands with responses, but represent read operations:
```csharp
public record GetUserQuery(int Id) : IRequest<UserModel>;
```

### Notifications
Multiple handlers can process the same notification:
```csharp
public record UserCreatedNotification(int UserId) : INotification;

// Multiple handlers can listen to the same notification
public class Handler1 : INotificationHandler<UserCreatedNotification> { }
public class Handler2 : INotificationHandler<UserCreatedNotification> { }
```

## Setup

All examples follow the same setup pattern:

```csharp
var services = new ServiceCollection();
services.AddFlowify(typeof(Program).Assembly);

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

// Use the mediator
await mediator.Send(command);
await mediator.Publish(notification);
```
