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
        private readonly IProductReadOnlyRepository _productReadOnlyRepository;

        public AddLineHandler(
            IOrderWriteRepository orderWriteRepository,
            IOrderReadOnlyRepository orderReadOnlyRepository,
            IUnitOfWork unitOfWork,
            IOrderLineValidator orderLineValidator,
            IProductReadOnlyRepository productReadOnlyRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadOnlyRepository = orderReadOnlyRepository;
            _unitOfWork = unitOfWork;
            _orderLineValidator = orderLineValidator;
            _productReadOnlyRepository = productReadOnlyRepository;
        }

        public async ValueTask<ResponseOrderDetailedDTO> Handle(
            AddLineCommand command,
            CancellationToken cancellationToken
        )
        {
            var request = command.Request;
            await ValidateAsync(request);
            var order = await _orderReadOnlyRepository.GetByIdAsync(command.OrderId);
            if (order is null)
                throw new NotFoundException(ResourceExceptionMessage.ORDER_NOT_FOUND);
            var product = await _productReadOnlyRepository.GetByIdAsync(request.ProductId);
            if (product is null)
                throw new NotFoundException(ResourceExceptionMessage.PRODUCT_NOT_FOUND);

            order.AddLine(request.ProductId, request.Quantity, request.Price);
            await _orderWriteRepository.AddLineAsync(order.Lines.Last());
            await _orderWriteRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return new ResponseOrderDetailedDTO(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.UpdatedAt,
                order.Lines.Select(x => new OrderLineDTO(
                    x.ProductId,
                    x.Quantity,
                    request.Price,
                    x.Total
                )).ToList(),
                order.Total
            );

        }
             async Task ValidateAsync(RequestOrderLineDTO request)
        {
            var validationResult = await _orderLineValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new OnValidationException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }
    }
}
