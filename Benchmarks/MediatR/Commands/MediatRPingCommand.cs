using MediatR;

namespace Benchmarks.MediatR.Commands;

public record MediatRPingCommand(string Message) : IRequest;
