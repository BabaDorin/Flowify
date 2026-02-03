using MediatR;

namespace Benchmarks.MediatR.Queries;

public class MediatRComplexQueryHandler : IRequestHandler<MediatRComplexQuery, MediatRComplexResult>
{
    public Task<MediatRComplexResult> Handle(MediatRComplexQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MediatRComplexResult
        {
            Id = request.Id,
            Data = $"Filtered: {request.Filter}",
            Count = (request.EndDate - request.StartDate).Days
        });
    }
}
