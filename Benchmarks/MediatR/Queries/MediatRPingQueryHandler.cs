using MediatR;

namespace Benchmarks.MediatR.Queries;

public class MediatRPingQueryHandler : IRequestHandler<MediatRPingQuery, string>
{
    public Task<string> Handle(MediatRPingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}
