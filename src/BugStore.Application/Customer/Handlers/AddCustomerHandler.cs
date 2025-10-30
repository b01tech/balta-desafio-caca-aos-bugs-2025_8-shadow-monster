using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.DTOs;
using BugStore.Domain.Interfaces;
using Mediator;

namespace BugStore.Application.Customer.Handlers;
public class AddCustomerHandler : IRequestHandler<AddCustomerCommand, ResponseDataCustomerDTO>
{
    private readonly ICustomerWriteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCustomerHandler(ICustomerWriteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<ResponseDataCustomerDTO> Handle(AddCustomerCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var customer = new Domain.Entities.Customer(request.Name, request.Email, request.Phone, request.BirthDate);

        await _repository.AddAsync(customer);
        await _unitOfWork.CommitAsync();

        return new ResponseDataCustomerDTO(customer.Id, customer.Name, customer.Email, customer.Phone, customer.BirthDate);
    }
}
