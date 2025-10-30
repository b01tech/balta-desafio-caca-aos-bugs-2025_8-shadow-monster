using BugStore.Application.Product.DTOs;
using Mediator;

namespace BugStore.Application.Product.Commands
{
    public class UpdatePriceCommand : IRequest<ResponseProductDetailedDTO>
    {
        public Guid Id;
        public decimal Price;

        public UpdatePriceCommand(Guid id, decimal price)
        {
            Id = id;
            Price = price;
        }
    }
}
