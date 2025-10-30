using BugStore.Application.Product.DTOs;
using BugStore.Application.Product.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class GetProductByIdHandler
        : IRequestHandler<GetProductByIdQuery, ResponseProductDetailedDTO>
    {
        private readonly IProductReadOnlyRepository _repository;

        public GetProductByIdHandler(IProductReadOnlyRepository repository)
        {
            _repository = repository;
        }

        public async ValueTask<ResponseProductDetailedDTO> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product is null)
                throw new NotFoundException(ResourceExceptionMessage.PRODUCT_NOT_FOUND);

            return new ResponseProductDetailedDTO(
                product.Id,
                product.Title,
                product.Description,
                product.Slug,
                product.Price
            );
        }
    }
}
