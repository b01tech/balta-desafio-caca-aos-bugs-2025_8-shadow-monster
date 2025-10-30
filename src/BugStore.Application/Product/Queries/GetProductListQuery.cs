using BugStore.Application.Product.DTOs;
using Mediator;

namespace BugStore.Application.Product.Queries
{
    public class GetProductListQuery : IRequest<ResponseListProductDTO>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetProductListQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
