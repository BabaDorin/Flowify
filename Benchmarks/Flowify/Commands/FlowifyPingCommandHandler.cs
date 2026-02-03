using Flowify.Contracts;

namespace Benchmarks.Flowify.Commands;

public class FlowifyPingCommandHandler : IRequestHandler<FlowifyPingCommand>
{
    public Task Handle(FlowifyPingCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
