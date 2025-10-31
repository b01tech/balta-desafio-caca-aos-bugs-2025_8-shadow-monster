using BugStore.Application.Product.Commands;
using BugStore.Application.Product.DTOs;
using BugStore.Application.Product.Validators;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, ResponseProductDetailedDTO>
    {
        private readonly IProductWriteRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductValidator _validator;

        public AddProductHandler(IProductWriteRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = new ProductValidator();
        }

        public async ValueTask<ResponseProductDetailedDTO> Handle(
            AddProductCommand command,
            CancellationToken cancellationToken
        )
        {
            var request = command.Request;

            await ValidateAsync(request);

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

        private async Task ValidateAsync(RequestProductDTO request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new OnValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage).ToList()
                );
            }
        }
    }
}
