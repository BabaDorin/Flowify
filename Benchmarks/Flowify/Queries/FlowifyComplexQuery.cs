using Flowify.Contracts;

namespace Benchmarks.Flowify.Queries;

public record FlowifyComplexQuery(int Id, string Filter, DateTime StartDate, DateTime EndDate) : IRequest<ComplexResult>;
