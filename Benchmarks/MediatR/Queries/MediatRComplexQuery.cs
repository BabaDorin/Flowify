using MediatR;

namespace Benchmarks.MediatR.Queries;

public record MediatRComplexQuery(int Id, string Filter, DateTime StartDate, DateTime EndDate) : IRequest<MediatRComplexResult>;
