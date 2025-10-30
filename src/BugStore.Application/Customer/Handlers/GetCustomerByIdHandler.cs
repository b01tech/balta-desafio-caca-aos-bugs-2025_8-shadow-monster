using BugStore.Application.Customer.DTOs;
using BugStore.Application.Customer.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Customer.Handlers
{
    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, ResponseDataCustomerDTO>
    {
        private readonly ICustomerReadOnlyRepository _repository;
        public GetCustomerByIdHandler(ICustomerReadOnlyRepository repository)
        {
            _repository = repository;
        }
        public async ValueTask<ResponseDataCustomerDTO> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _repository.GetByIdAsync(request.Id);
            if (customer is null)
                throw new NotFoundException(ResourceExceptionMessage.CUSTOMER_NOT_FOUND);

            return new ResponseDataCustomerDTO(
                customer.Id,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.BirthDate);
        }
    }
}
