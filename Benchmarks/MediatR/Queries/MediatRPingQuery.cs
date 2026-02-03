using MediatR;

namespace Benchmarks.MediatR.Queries;

public record MediatRPingQuery(string Message) : IRequest<string>;
