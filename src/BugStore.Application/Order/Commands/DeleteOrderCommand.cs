using Mediator;

namespace BugStore.Application.Order.Commands
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }
    }
}
