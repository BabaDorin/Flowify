namespace Flowify.Tests.TestFixtures.Commands;

public class TestCommandWithoutResponseHandler : IRequestHandler<TestCommandWithoutResponse>
{
    public List<string> HandledMessages { get; } = new();

    public Task Handle(TestCommandWithoutResponse request, CancellationToken cancellationToken)
    {
        HandledMessages.Add(request.Message);
        return Task.CompletedTask;
    }
}
