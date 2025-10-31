using BugStore.Application.Reports.DTOs;
using BugStore.Application.Reports.Queries;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Reports.Handlers
{
    public class GetRevenueByCustomerHandler : IRequestHandler<GetRevenueByCustomerQuery, ResponseRevenueByCustomerDTO>
    {
        private readonly IOrderReadOnlyRepository _orderRepository;
        private readonly ICustomerReadOnlyRepository _customerRepository;

        public GetRevenueByCustomerHandler(IOrderReadOnlyRepository orderRepository, ICustomerReadOnlyRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        public async ValueTask<ResponseRevenueByCustomerDTO> Handle(GetRevenueByCustomerQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Request.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Cliente com ID {request.Request.CustomerId} não encontrado.");
            }

            var (totalOrders, totalSpent) = await _orderRepository.GetTotalByCustomerIdAsync(request.Request.CustomerId);

            return new ResponseRevenueByCustomerDTO(
                customer.Id,
                customer.Name,
                totalOrders,
                totalSpent);
        }
    }
}