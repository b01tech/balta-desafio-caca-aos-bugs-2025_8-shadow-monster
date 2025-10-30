using BugStore.Application.Product.Commands;
using BugStore.Application.Product.DTOs;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, ResponseProductDetailedDTO>
    {
        private readonly IProductWriteRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductHandler(IProductWriteRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<ResponseProductDetailedDTO> Handle(
            AddProductCommand command,
            CancellationToken cancellationToken
        )
        {
            var request = command.Request;
            // validar
            var product = new Domain.Entities.Product(
                request.Title,
                request.Description,
                request.Slug,
                request.Price
            );
            await _repository.AddAsync(product);
            await _unitOfWork.CommitAsync();

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
