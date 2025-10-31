using BugStore.Application.Product.Commands;
using BugStore.Application.Product.DTOs;
using BugStore.Application.Product.Validators;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class UpdateProductHandler
        : IRequestHandler<UpdateProductCommand, ResponseProductDetailedDTO>
    {
        private readonly IProductReadOnlyRepository _readRepository;
        private readonly IProductWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(
            IProductReadOnlyRepository readRepository,
            IProductWriteRepository writeRepository,
            IUnitOfWork unitOfWork
        )
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<ResponseProductDetailedDTO> Handle(
            UpdateProductCommand command,
            CancellationToken cancellationToken
        )
        {
            var request = command.Request;

            // Validar dados de entrada
            await ValidateAsync(request);

            var product = await _readRepository.GetByIdAsync(command.Id);
            if (product is null)
                throw new NotFoundException(ResourceExceptionMessage.PRODUCT_NOT_FOUND);

            product.Update(request.Title, request.Description, request.Slug, request.Price);
            await _writeRepository.UpdateAsync(product);
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
            var validator = new ProductValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new OnValidationException(
                    validationResult.Errors.Select(x => x.ErrorMessage).ToList()
                );
            }
        }
    }
}
