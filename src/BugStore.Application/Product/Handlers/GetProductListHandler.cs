using BugStore.Application.Extensions;
using BugStore.Application.Product.DTOs;
using BugStore.Application.Product.Queries;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class GetProductListHandler
        : IRequestHandler<GetProductListQuery, ResponseListProductDTO>
    {
        private readonly IProductReadOnlyRepository _repository;

        public GetProductListHandler(IProductReadOnlyRepository repository)
        {
            _repository = repository;
        }

        public async ValueTask<ResponseListProductDTO> Handle(
            GetProductListQuery request,
            CancellationToken cancellationToken
        )
        {
            var products = await _repository.GetAllAsync(request.Page, request.PageSize);
            var totalProducts = await _repository.GetTotalItemAsync();

            return new ResponseListProductDTO(
                totalProducts,
                request.Page,
                totalProducts.CalculateTotalPages(request.PageSize),
                products.Select(x => new ResponseProductDTO(x.Id, x.Title, x.Price)).ToList()
            );
        }
    }
}
