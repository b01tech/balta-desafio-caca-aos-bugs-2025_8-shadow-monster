using BugStore.Application.Order.Commands;
using BugStore.Application.Order.DTOs;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Order.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, ResponseOrderSummaryDTO>
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(
            IOrderWriteRepository orderWriteRepository,
            IOrderReadOnlyRepository orderReadOnlyRepository,
            IUnitOfWork unitOfWork
        )
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadOnlyRepository = orderReadOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<ResponseOrderSummaryDTO> Handle(
            CreateOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            var customerId = command.CustomerId;
            var order = new Domain.Entities.Order(customerId);

            await _orderWriteRepository.AddAsync(order);
            await _unitOfWork.CommitAsync();

            var createdOrder = await _orderReadOnlyRepository.GetByIdAsync(order.Id);

            return new ResponseOrderSummaryDTO(
                createdOrder!.Id,
                createdOrder.CustomerId,
                createdOrder.CreatedAt,
                createdOrder.Total
            );
        }
    }
}
