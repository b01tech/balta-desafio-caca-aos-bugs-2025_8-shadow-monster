using BugStore.Application.Product.DTOs;
using Mediator;

namespace BugStore.Application.Product.Queries
{
    public class GetProductByIdQuery : IRequest<ResponseProductDetailedDTO>
    {
        public Guid Id { get; set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
