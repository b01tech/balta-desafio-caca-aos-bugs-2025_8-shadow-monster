using BugStore.Application.Product.Commands;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Product.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IProductWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Unit> Handle(
            DeleteProductCommand command,
            CancellationToken cancellationToken
        )
        {
            await _writeRepository.DeleteAsync(command.Id);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
