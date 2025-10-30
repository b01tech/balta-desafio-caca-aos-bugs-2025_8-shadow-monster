using Mediator;

namespace BugStore.Application.Customer.Commands;
public class DeleteCustomerCommand : IRequest
{
    public Guid Id { get; set; }
    public DeleteCustomerCommand(Guid id)
    {
        Id = id;
    }
}
