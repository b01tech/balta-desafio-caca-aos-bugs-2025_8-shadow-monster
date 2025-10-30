using BugStore.Application.Customer.DTOs;
using Mediator;

namespace BugStore.Application.Customer.Queries;
public class GetCustomerListQuery : IRequest<ResponseListCustomerDTO>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetCustomerListQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

}
