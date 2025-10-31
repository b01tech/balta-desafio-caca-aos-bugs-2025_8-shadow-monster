using BugStore.Application.Order.DTOs;
using BugStore.Application.Order.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Order.Handlers
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, ResponseOrderDetailedDTO>
    {
        private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;

        public GetOrderByIdHandler(IOrderReadOnlyRepository orderReadOnlyRepository)
        {
            _orderReadOnlyRepository = orderReadOnlyRepository;
        }

        public async ValueTask<ResponseOrderDetailedDTO> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var order = await _orderReadOnlyRepository.GetByIdAsync(request.Id);

            if (order is null)
                throw new NotFoundException(ResourceExceptionMessage.ORDER_NOT_FOUND);

            return new ResponseOrderDetailedDTO(
                order!.Id,
                order.CustomerId,
                order.CreatedAt,
                order.UpdatedAt,
                order
                    .Lines.Select(l => new OrderLineDTO(
                        l.ProductId,
                        l.Quantity,
                        l.Product.Price,
                        l.Total
                    ))
                    .ToList(),
                order.Total
            );
        }
    }
}
