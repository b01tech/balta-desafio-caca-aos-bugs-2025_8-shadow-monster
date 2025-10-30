using BugStore.Application.Product.DTOs;
using Mediator;

namespace BugStore.Application.Product.Commands
{
    public class AddProductCommand : IRequest<ResponseProductDetailedDTO>
    {
        public RequestProductDTO Request;

        public AddProductCommand(RequestProductDTO request)
        {
            Request = request;
        }
    }
}
