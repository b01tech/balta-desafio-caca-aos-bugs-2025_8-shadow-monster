using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Queries
{
    public class GetOrderByCustomerIdQuery : IRequest<ResponseListOrderDTO>
    {
        public Guid CustomerId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetOrderByCustomerIdQuery(Guid customerId, int page, int pageSize)
        {
            CustomerId = customerId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
