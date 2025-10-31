using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Commands
{
    public class CreateOrderCommand : IRequest<ResponseOrderSummaryDTO>
    {
        public Guid CustomerId { get; set; }

        public CreateOrderCommand(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
