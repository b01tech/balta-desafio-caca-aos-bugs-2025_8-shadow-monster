using BugStore.Application.Reports.DTOs;
using BugStore.Application.Reports.Queries;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Reports.Handlers
{
    public class GetRevenueByPeriodHandler : IRequestHandler<GetRevenueByPediodQuery, ResponseRevenueByPeriodDTO>
    {
        private readonly IOrderReadOnlyRepository _orderRepository;

        public GetRevenueByPeriodHandler(IOrderReadOnlyRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async ValueTask<ResponseRevenueByPeriodDTO> Handle(GetRevenueByPediodQuery request, CancellationToken cancellationToken)
        {
            var (totalOrders, totalRevenue) = await _orderRepository.GetTotalByPeriod(
                request.Request.StartDate, 
                request.Request.EndDate);

            return new ResponseRevenueByPeriodDTO(
                request.Request.StartDate,
                request.Request.EndDate,
                totalOrders,
                totalRevenue);
        }
    }
}