using Flowify.Contracts;

namespace Benchmarks.Flowify.Queries;

public record FlowifyPingQuery(string Message) : IRequest<string>;
