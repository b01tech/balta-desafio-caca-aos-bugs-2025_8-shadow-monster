using BugStore.Application.Extensions;
using BugStore.Application.Order.DTOs;
using BugStore.Application.Order.Queries;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Order.Handlers
{
    public class GetOrderListHandler : IRequestHandler<GetOrderListQuery, ResponseListOrderDTO>
    {
        private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;

        public GetOrderListHandler(IOrderReadOnlyRepository orderReadOnlyRepository)
        {
            _orderReadOnlyRepository = orderReadOnlyRepository;
        }

        public async ValueTask<ResponseListOrderDTO> Handle(
            GetOrderListQuery request,
            CancellationToken cancellationToken
        )
        {
            var orders = await _orderReadOnlyRepository.GetAllAsync(request.Page, request.PageSize);
            var totalItems = await _orderReadOnlyRepository.GetTotalItemAsync();

            var totalPages = totalItems.CalculateTotalPages(request.PageSize);

            var orderSummaries = orders
                .Select(o => new ResponseOrderSummaryDTO(o.Id, o.CustomerId, o.CreatedAt, o.Total))
                .ToList();

            return new ResponseListOrderDTO(totalItems, request.Page, totalPages, orderSummaries);
        }
    }
}
