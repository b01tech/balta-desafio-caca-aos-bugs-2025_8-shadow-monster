using Mediator;

namespace BugStore.Application.Services.Mediator
{
    public class MediatorService : IMediatorService
    {
        private readonly IMediator _mediator;
        public MediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<TResponse?> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request!, cancellationToken);
            return (TResponse?)response;
        }
    }
}
