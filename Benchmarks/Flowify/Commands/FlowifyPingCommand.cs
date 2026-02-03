using Flowify.Contracts;

namespace Benchmarks.Flowify.Commands;

public record FlowifyPingCommand(string Message) : IRequest;
