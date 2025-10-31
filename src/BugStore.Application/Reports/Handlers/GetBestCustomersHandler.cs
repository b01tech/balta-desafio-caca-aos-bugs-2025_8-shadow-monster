using BugStore.Application.Reports.DTOs;
using BugStore.Application.Reports.Queries;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Reports.Handlers
{
    public class GetBestCustomersHandler : IRequestHandler<GetBestCustomersQuery, ResponseBestCustomersListDTO>
    {
        private readonly IOrderReadOnlyRepository _orderRepository;

        public GetBestCustomersHandler(IOrderReadOnlyRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async ValueTask<ResponseBestCustomersListDTO> Handle(GetBestCustomersQuery request, CancellationToken cancellationToken)
        {
            var bestCustomers = await _orderRepository.GetBestCustomersAsync(request.Request.TopCustomers);

            var customerDTOs = bestCustomers.Select(bc => new ResponseBestCustomerDTO(
                bc.CustomerId,
                bc.CustomerName,
                bc.TotalOrders,
                bc.TotalSpent)).ToList();

            return new ResponseBestCustomersListDTO(customerDTOs);
        }
    }
}