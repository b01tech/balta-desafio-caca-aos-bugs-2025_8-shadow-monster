using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Commands
{
    public class AddLineCommand : IRequest<ResponseOrderDetailedDTO>
    {
        public Guid OrderId { get; set; }
        public RequestOrderLineDTO Request { get; set; }

        public AddLineCommand(Guid orderId, RequestOrderLineDTO request)
        {
            OrderId = orderId;
            Request = request;
        }
    }
}
