using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.DTOs;
using BugStore.Application.Customer.Validators;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;
using Mediator;

namespace BugStore.Application.Customer.Handlers;

public class AddCustomerHandler : IRequestHandler<AddCustomerCommand, ResponseDataCustomerDTO>
{
    private readonly ICustomerReadOnlyRepository _readRepository;
    private readonly ICustomerWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCustomerHandler(
        ICustomerReadOnlyRepository readRepository,
        ICustomerWriteRepository writeRepository,
        IUnitOfWork unitOfWork
    )
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<ResponseDataCustomerDTO> Handle(
        AddCustomerCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = command.Request;
        await ValidateAsync(request);
        var customer = new Domain.Entities.Customer(
            request.Name,
            request.Email,
            request.Phone,
            request.BirthDate
        );

        await _writeRepository.AddAsync(customer);
        await _unitOfWork.CommitAsync();

        return new ResponseDataCustomerDTO(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.BirthDate
        );
    }

    private async Task ValidateAsync(RequestCustomerDTO request)
    {
        var validator = new CustomerValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new OnValidationException(
                validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            );
        var exists = await _readRepository.ExistsByEmailAsync(request.Email);
        if (exists)
            throw new ConflitException(ResourceExceptionMessage.EMAIL_ALREADY_REGISTERED);
    }
}
