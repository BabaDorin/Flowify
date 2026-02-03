using Flowify.Contracts;

namespace Benchmarks.Flowify.Queries;

public class FlowifyPingQueryHandler : IRequestHandler<FlowifyPingQuery, string>
{
    public Task<string> Handle(FlowifyPingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}
