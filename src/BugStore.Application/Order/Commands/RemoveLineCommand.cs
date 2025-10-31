using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Commands
{
    public class RemoveLineCommand : IRequest<ResponseOrderDetailedDTO>
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public RemoveLineCommand(Guid orderId, Guid productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }
    }
}
