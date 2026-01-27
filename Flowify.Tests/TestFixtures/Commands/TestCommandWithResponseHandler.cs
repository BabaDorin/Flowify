using Flowify.Contracts;

namespace Flowify.Tests.TestFixtures.Commands;

public class TestCommandWithResponseHandler : IRequestHandler<TestCommandWithResponse, int>
{
    public Task<int> Handle(TestCommandWithResponse request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Value.Length);
    }
}
