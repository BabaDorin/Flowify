using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Flowify.Tests.TestFixtures.Commands;
using Flowify.Tests.TestFixtures.Queries;
using Flowify.Tests.TestFixtures.Notifications;

namespace Flowify.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFlowify_RegistersMediator()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetService<IMediator>();

        mediator.ShouldNotBeNull();
        mediator.ShouldBeOfType<Mediator>();
    }

    [Fact]
    public void AddFlowify_RegistersRequestHandlersWithResponse()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var commandHandler = serviceProvider.GetService<IRequestHandler<TestCommandWithResponse, int>>();
        var queryHandler = serviceProvider.GetService<IRequestHandler<TestQuery, string>>();

        commandHandler.ShouldNotBeNull();
        commandHandler.ShouldBeOfType<TestCommandWithResponseHandler>();
        queryHandler.ShouldNotBeNull();
        queryHandler.ShouldBeOfType<TestQueryHandler>();
    }

    [Fact]
    public void AddFlowify_RegistersRequestHandlersWithoutResponse()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithoutResponse).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var commandHandler = serviceProvider.GetService<IRequestHandler<TestCommandWithoutResponse>>();

        commandHandler.ShouldNotBeNull();
        commandHandler.ShouldBeOfType<TestCommandWithoutResponseHandler>();
    }

    [Fact]
    public void AddFlowify_RegistersNotificationHandlers()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestNotification).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var handlers = serviceProvider.GetServices<INotificationHandler<TestNotification>>().ToList();

        handlers.ShouldNotBeEmpty();
        handlers.Count.ShouldBe(2);
        handlers.ShouldContain(h => h is TestNotificationHandler1);
        handlers.ShouldContain(h => h is TestNotificationHandler2);
    }

    [Fact]
    public void AddFlowify_WithNoAssemblies_UsesCallingAssembly()
    {
        var services = new ServiceCollection();
        services.AddFlowify();
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetService<IMediator>();

        mediator.ShouldNotBeNull();
    }

    [Fact]
    public void AddFlowify_WithMultipleAssemblies_RegistersHandlersFromAllAssemblies()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly, typeof(ServiceCollectionExtensions).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var commandHandler = serviceProvider.GetService<IRequestHandler<TestCommandWithResponse, int>>();

        commandHandler.ShouldNotBeNull();
    }

    [Fact]
    public void AddFlowify_RegistersHandlersAsTransient()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);

        var commandHandlerDescriptor = services.FirstOrDefault(d => 
            d.ServiceType == typeof(IRequestHandler<TestCommandWithResponse, int>));

        commandHandlerDescriptor.ShouldNotBeNull();
        commandHandlerDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddFlowify_RegistersMediatorAsTransient()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);

        var mediatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));

        mediatorDescriptor.ShouldNotBeNull();
        mediatorDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }

    [Fact]
    public async Task AddFlowify_IntegrationTest_CanSendCommandsWithResponse()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var commandResult = await mediator.Send(new TestCommandWithResponse("Testing"));

        commandResult.ShouldBe(7);
    }

    [Fact]
    public async Task AddFlowify_IntegrationTest_CanSendCommandsWithoutResponse()
    {
        var handler = new TestCommandWithoutResponseHandler();
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithoutResponse).Assembly);
        services.AddSingleton<IRequestHandler<TestCommandWithoutResponse>>(handler);
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new TestCommandWithoutResponse("Integration Test"));

        handler.HandledMessages.ShouldContain("Integration Test");
    }

    [Fact]
    public async Task AddFlowify_IntegrationTest_CanSendQueries()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestQuery).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var queryResult = await mediator.Send(new TestQuery(123));

        queryResult.ShouldBe("Result for ID: 123");
    }

    [Fact]
    public async Task AddFlowify_IntegrationTest_CanPublishNotifications()
    {
        var handler1 = new TestNotificationHandler1();
        var handler2 = new TestNotificationHandler2();

        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestNotification).Assembly);
        
        services.AddSingleton<INotificationHandler<TestNotification>>(handler1);
        services.AddSingleton<INotificationHandler<TestNotification>>(handler2);

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(new TestNotification("Integration Test"));

        handler1.HandledMessages.ShouldContain("Handler1: Integration Test");
        handler2.HandledMessages.ShouldContain("Handler2: Integration Test");
    }

    [Fact]
    public void AddFlowify_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        
        var result = services.AddFlowify(typeof(TestCommandWithResponse).Assembly);

        result.ShouldBe(services);
    }

    [Fact]
    public void AddFlowify_DoesNotRegisterAbstractClasses()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);

        var abstractHandlers = services.Where(d => 
            d.ImplementationType?.IsAbstract == true).ToList();

        abstractHandlers.ShouldBeEmpty();
    }

    [Fact]
    public void AddFlowify_DoesNotRegisterInterfaces()
    {
        var services = new ServiceCollection();
        services.AddFlowify(typeof(TestCommandWithResponse).Assembly);

        var interfaceHandlers = services.Where(d => 
            d.ImplementationType?.IsInterface == true).ToList();

        interfaceHandlers.ShouldBeEmpty();
    }
}
