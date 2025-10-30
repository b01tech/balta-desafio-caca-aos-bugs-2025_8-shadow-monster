using BugStore.Application.Customer.DTOs;
using Mediator;

namespace BugStore.Application.Customer.Queries;
public class GetCustomerByIdQuery : IRequest<ResponseDataCustomerDTO>
{
    public Guid Id { get; set; }
    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }
}
