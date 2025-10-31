using BugStore.Application.Order.Commands;
using BugStore.Application.Order.DTOs;
using BugStore.Application.Order.Validators;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Order.Handlers
{
    public class AddLineHandler : IRequestHandler<AddLineCommand, ResponseOrderDetailedDTO>
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderLineValidator _orderLineValidator;

        public AddLineHandler(
            IOrderWriteRepository orderWriteRepository,
            IOrderReadOnlyRepository orderReadOnlyRepository,
            IUnitOfWork unitOfWork,
            IOrderLineValidator orderLineValidator
        )
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadOnlyRepository = orderReadOnlyRepository;
            _unitOfWork = unitOfWork;
            _orderLineValidator = orderLineValidator;
        }

        public async ValueTask<ResponseOrderDetailedDTO> Handle(
            AddLineCommand command,
            CancellationToken cancellationToken
        )
        {
            await ValidateAsync(command.Request);

            var order = await _orderReadOnlyRepository.GetByIdAsync(command.OrderId);
            if (order is null)
                throw new NotFoundException(ResourceExceptionMessage.ORDER_NOT_FOUND);

            order!.AddLine(
                command.Request.ProductId,
                command.Request.Quantity,
                command.Request.Price
            );

            await _orderWriteRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            var updatedOrder = await _orderReadOnlyRepository.GetByIdAsync(command.OrderId);

            return new ResponseOrderDetailedDTO(
                updatedOrder!.Id,
                updatedOrder.CustomerId,
                updatedOrder.CreatedAt,
                updatedOrder.UpdatedAt,
                updatedOrder
                    .Lines.Select(l => new OrderLineDTO(
                        l.ProductId,
                        l.Quantity,
                        l.Product.Price,
                        l.Total
                    ))
                    .ToList(),
                updatedOrder.Total
            );
        }

        private async Task ValidateAsync(RequestOrderLineDTO request)
        {
            var validationResult = await _orderLineValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new OnValidationException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }
    }
}
