using BugStore.Application.Product.DTOs;
using Mediator;

namespace BugStore.Application.Product.Commands
{
    public class UpdateProductCommand : IRequest<ResponseProductDetailedDTO>
    {
        public Guid Id;
        public RequestProductDTO Request;

        public UpdateProductCommand(Guid id, RequestProductDTO request)
        {
            Id = id;
            Request = request;
        }
    }
}
