using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Queries
{
    public class GetOrderListQuery : IRequest<ResponseListOrderDTO>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetOrderListQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
