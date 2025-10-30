namespace BugStore.Application.Services.Mediator
{
    public interface IMediatorService
    {
        Task<TResponse?> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);
    }
}
