namespace Flowify.Contracts
{
    public interface IMediator
    {
        Task Send(IRequest request, CancellationToken cancellationToken = default);

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}
