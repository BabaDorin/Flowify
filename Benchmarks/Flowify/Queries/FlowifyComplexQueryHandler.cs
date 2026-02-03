using Flowify.Contracts;

namespace Benchmarks.Flowify.Queries;

public class FlowifyComplexQueryHandler : IRequestHandler<FlowifyComplexQuery, ComplexResult>
{
    public Task<ComplexResult> Handle(FlowifyComplexQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ComplexResult
        {
            Id = request.Id,
            Data = $"Filtered: {request.Filter}",
            Count = (request.EndDate - request.StartDate).Days
        });
    }
}
