using BugStore.Application.Product.Commands;
using BugStore.Application.Product.DTOs;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class UpdatePriceHandler
        : IRequestHandler<UpdatePriceCommand, ResponseProductDetailedDTO>
    {
        private readonly IProductReadOnlyRepository _readRepository;
        private readonly IProductWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePriceHandler(
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
            UpdatePriceCommand command,
            CancellationToken cancellationToken
        )
        {
            var product = await _readRepository.GetByIdAsync(command.Id);
            if (product is null)
                throw new NotFoundException(ResourceExceptionMessage.PRODUCT_NOT_FOUND);

            product.UpdatePrice(command.Price);
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
    }
}
