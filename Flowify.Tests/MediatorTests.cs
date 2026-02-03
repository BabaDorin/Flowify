using AutoFixture;
using AutoFixture.AutoMoq;
using Flowify.Contracts;
using Flowify.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Flowify.Tests.TestFixtures.Commands;
using Flowify.Tests.TestFixtures.Queries;
using Flowify.Tests.TestFixtures.Notifications;

namespace Flowify.Tests;

public class MediatorTests
{
    private readonly IFixture _fixture;

    public MediatorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task Send_WithValidCommandWithResponse_ReturnsExpectedResult()
    {
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestCommandWithResponse, int>, TestCommandWithResponseHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithResponse("Hello");
        var result = await mediator.Send(command);

        result.ShouldBe(5);
    }

    [Fact]
    public async Task Send_WithValidCommandWithoutResponse_ExecutesSuccessfully()
    {
        var handler = new TestCommandWithoutResponseHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IRequestHandler<TestCommandWithoutResponse>>(handler);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithoutResponse("Test Message");
        await mediator.Send(command);

        handler.HandledMessages.ShouldContain("Test Message");
    }

    [Fact]
    public async Task Send_WithValidQuery_ReturnsExpectedResult()
    {
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var query = new TestQuery(42);
        var result = await mediator.Send(query);

        result.ShouldBe("Result for ID: 42");
    }

    [Fact]
    public async Task Send_WithNullRequestWithResponse_ThrowsArgumentNullException()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        await Should.ThrowAsync<ArgumentNullException>(async () => 
            await mediator.Send<int>(null!));
    }

    [Fact]
    public async Task Send_WithNullRequestWithoutResponse_ThrowsArgumentNullException()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        await Should.ThrowAsync<ArgumentNullException>(async () => 
            await mediator.Send((IRequest)null!));
    }

    [Fact]
    public async Task Send_WithNoHandlerRegisteredForResponseCommand_ThrowsInvalidOperationException()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithResponse("Test");

        var exception = await Should.ThrowAsync<InvalidOperationException>(async () => 
            await mediator.Send(command));

        exception.Message.ShouldContain("No handler registered");
    }

    [Fact]
    public async Task Send_WithNoHandlerRegisteredForVoidCommand_ThrowsInvalidOperationException()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithoutResponse("Test");

        var exception = await Should.ThrowAsync<InvalidOperationException>(async () => 
            await mediator.Send(command));

        exception.Message.ShouldContain("No handler registered");
    }

    [Fact]
    public async Task Send_WithMockedHandlerWithResponse_CallsHandleMethod()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithResponse, int>>();
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithResponse("Test");
        var result = await mediator.Send(command);

        result.ShouldBe(100);
        mockHandler.Verify(h => h.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_WithMockedHandlerWithoutResponse_CallsHandleMethod()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithoutResponse>>();
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithoutResponse>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithoutResponse("Test");
        await mediator.Send(command);

        mockHandler.Verify(h => h.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_WithCancellationTokenForResponseCommand_PassesTokenToHandler()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithResponse, int>>();
        var cancellationToken = new CancellationToken();

        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithResponse>(), cancellationToken))
            .ReturnsAsync(42);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithResponse("Test");
        await mediator.Send(command, cancellationToken);

        mockHandler.Verify(h => h.Handle(command, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Send_WithCancellationTokenForVoidCommand_PassesTokenToHandler()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithoutResponse>>();
        var cancellationToken = new CancellationToken();

        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithoutResponse>(), cancellationToken))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var command = new TestCommandWithoutResponse("Test");
        await mediator.Send(command, cancellationToken);

        mockHandler.Verify(h => h.Handle(command, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Publish_WithNullNotification_ThrowsArgumentNullException()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        await Should.ThrowAsync<ArgumentNullException>(async () => 
            await mediator.Publish<TestNotification>(null!));
    }

    [Fact]
    public async Task Publish_WithNoHandlers_CompletesSuccessfully()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var notification = new TestNotification("Test");

        await Should.NotThrowAsync(async () => 
            await mediator.Publish(notification));
    }

    [Fact]
    public async Task Publish_WithSingleHandler_InvokesHandler()
    {
        var handler = new TestNotificationHandler1();
        var services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<TestNotification>>(handler);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var notification = new TestNotification("Hello");
        await mediator.Publish(notification);

        handler.HandledMessages.ShouldContain("Handler1: Hello");
        handler.HandledMessages.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Publish_WithMultipleHandlers_InvokesAllHandlers()
    {
        var handler1 = new TestNotificationHandler1();
        var handler2 = new TestNotificationHandler2();
        
        var services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<TestNotification>>(handler1);
        services.AddSingleton<INotificationHandler<TestNotification>>(handler2);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var notification = new TestNotification("Broadcast");
        await mediator.Publish(notification);

        handler1.HandledMessages.ShouldContain("Handler1: Broadcast");
        handler2.HandledMessages.ShouldContain("Handler2: Broadcast");
    }

    [Fact]
    public async Task Publish_WithMockedHandler_CallsHandleMethod()
    {
        var mockHandler = new Mock<INotificationHandler<TestNotification>>();
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var notification = new TestNotification("Test");
        await mediator.Publish(notification);

        mockHandler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_WithCancellationToken_PassesTokenToHandlers()
    {
        var mockHandler = new Mock<INotificationHandler<TestNotification>>();
        var cancellationToken = new CancellationToken();

        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestNotification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var notification = new TestNotification("Test");
        await mediator.Publish(notification, cancellationToken);

        mockHandler.Verify(h => h.Handle(notification, cancellationToken), Times.Once);
    }

    [Fact]
    public void Mediator_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new Mediator(null!));
    }

    [Fact]
    public async Task Send_WhenHandlerThrowsException_PropagatesOriginalException()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithResponse, int>>();
        var expectedException = new InvalidOperationException("Handler failed");
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithResponse>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var exception = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await mediator.Send(new TestCommandWithResponse("Test")));

        exception.Message.ShouldBe("Handler failed");
    }

    [Fact]
    public async Task Send_WithoutResponse_WhenHandlerThrowsException_PropagatesOriginalException()
    {
        var mockHandler = new Mock<IRequestHandler<TestCommandWithoutResponse>>();
        var expectedException = new ArgumentException("Invalid argument");
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestCommandWithoutResponse>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var exception = await Should.ThrowAsync<ArgumentException>(async () =>
            await mediator.Send(new TestCommandWithoutResponse("Test")));

        exception.Message.ShouldBe("Invalid argument");
    }

    [Fact]
    public async Task Publish_WhenSingleHandlerThrows_PropagatesException()
    {
        var mockHandler = new Mock<INotificationHandler<TestNotification>>();
        var expectedException = new InvalidOperationException("Handler failed");
        mockHandler
            .Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        var exception = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await mediator.Publish(new TestNotification("Test")));

        exception.Message.ShouldBe("Handler failed");
    }

    [Fact]
    public async Task Publish_WhenMultipleHandlersThrow_ThrowsFirstException()
    {
        var mockHandler1 = new Mock<INotificationHandler<TestNotification>>();
        var mockHandler2 = new Mock<INotificationHandler<TestNotification>>();

        mockHandler1
            .Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Handler 1 failed"));
        mockHandler2
            .Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Handler 2 failed"));

        var services = new ServiceCollection();
        services.AddSingleton(mockHandler1.Object);
        services.AddSingleton(mockHandler2.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        await Should.ThrowAsync<Exception>(async () =>
            await mediator.Publish(new TestNotification("Test")));
    }
}
