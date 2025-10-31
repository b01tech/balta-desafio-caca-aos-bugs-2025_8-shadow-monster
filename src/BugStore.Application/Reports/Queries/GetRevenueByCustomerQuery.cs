using BugStore.Application.Reports.DTOs;
using Mediator;

namespace BugStore.Application.Reports.Queries
{
    public class GetRevenueByCustomerQuery : IRequest<ResponseRevenueByCustomerDTO>
    {
        public RequestRevenueByCustomerDTO Request { get; set; }
        public GetRevenueByCustomerQuery(RequestRevenueByCustomerDTO request)
        {
            Request = request;
        }
    }
}
