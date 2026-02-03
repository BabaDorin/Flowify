using MediatR;

namespace Benchmarks.MediatR.Commands;

public class MediatRPingCommandHandler : IRequestHandler<MediatRPingCommand>
{
    public Task Handle(MediatRPingCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
