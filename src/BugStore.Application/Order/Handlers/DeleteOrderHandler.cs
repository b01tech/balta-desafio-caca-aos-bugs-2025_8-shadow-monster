using BugStore.Application.Order.Commands;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Order.Handlers
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderHandler(
            IOrderWriteRepository orderWriteRepository,
            IUnitOfWork unitOfWork
        )
        {
            _orderWriteRepository = orderWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Unit> Handle(
            DeleteOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            await _orderWriteRepository.DeleteAsync(command.Id);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
