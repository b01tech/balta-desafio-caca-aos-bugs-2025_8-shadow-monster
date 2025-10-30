using BugStore.Application.Customer.Commands;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Customer.Handlers
{
    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand>
    {
        private readonly ICustomerWriteRepository _writeRepository;
        private readonly ICustomerReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerHandler(ICustomerWriteRepository writeRepository, ICustomerReadOnlyRepository readOnlyRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _readOnlyRepository = readOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _readOnlyRepository.GetByIdAsync(request.Id);
            if (customer is null)
                throw new NotFoundException(ResourceExceptionMessage.CUSTOMER_NOT_FOUND);

            await _writeRepository.DeleteAsync(request.Id);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
