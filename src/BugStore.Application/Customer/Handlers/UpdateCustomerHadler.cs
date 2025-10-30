using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.DTOs;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Customer.Handlers
{
    public class UpdateCustomerHadler : IRequestHandler<UpdateCustomerCommand, ResponseDataCustomerDTO>
    {
        private readonly ICustomerWriteRepository _writeRepository;
        private readonly ICustomerReadOnlyRepository _readOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCustomerHadler(ICustomerWriteRepository writeRepository, ICustomerReadOnlyRepository readOnlyRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _readOnlyRepository = readOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<ResponseDataCustomerDTO> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var customer = await _readOnlyRepository.GetByIdAsync(command.Id);
            if (customer is null)
                throw new NotFoundException(ResourceExceptionMessage.CUSTOMER_NOT_FOUND);

            customer.Update(request.Name, request.Email, request.Phone, request.BirthDate);
            await _writeRepository.UpdateAsync(customer);
            await _unitOfWork.CommitAsync();
            return new ResponseDataCustomerDTO(customer.Id, customer.Name, customer.Email, customer.Phone, customer.BirthDate);
        }
    }
}
