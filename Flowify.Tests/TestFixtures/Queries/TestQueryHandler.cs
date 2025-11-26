namespace Flowify.Tests.TestFixtures.Queries;

public class TestQueryHandler : IRequestHandler<TestQuery, string>
{
    public Task<string> Handle(TestQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Result for ID: {request.Id}");
    }
}
