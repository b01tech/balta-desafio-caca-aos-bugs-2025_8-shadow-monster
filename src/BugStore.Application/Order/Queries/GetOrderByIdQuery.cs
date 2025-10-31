using BugStore.Application.Order.DTOs;
using Mediator;

namespace BugStore.Application.Order.Queries
{
    public class GetOrderByIdQuery : IRequest<ResponseOrderDetailedDTO>
    {
        public Guid Id { get; set; }

        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
