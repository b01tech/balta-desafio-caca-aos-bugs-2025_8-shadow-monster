using BugStore.Application.Reports.DTOs;
using Mediator;

namespace BugStore.Application.Reports.Queries
{
    public class GetRevenueByPediodQuery : IRequest<ResponseRevenueByPeriodDTO>
    {
        public RequestRevenueByPeriodDTO Request { get; set; }

        public GetRevenueByPediodQuery(RequestRevenueByPeriodDTO request)
        {
            Request = request;
        }
    }
}
