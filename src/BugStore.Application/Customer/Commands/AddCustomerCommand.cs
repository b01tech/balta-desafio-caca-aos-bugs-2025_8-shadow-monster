using BugStore.Application.Customer.DTOs;
using Mediator;

namespace BugStore.Application.Customer.Commands;
public class AddCustomerCommand : IRequest<ResponseDataCustomerDTO>
{
    public RequestCustomerDTO Request { get; set; }
    public AddCustomerCommand(RequestCustomerDTO request)
    {
        Request = request;
    }
}
