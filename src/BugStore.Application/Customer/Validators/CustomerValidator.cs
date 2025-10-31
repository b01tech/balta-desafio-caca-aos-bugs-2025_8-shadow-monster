using BugStore.Application.Customer.DTOs;
using BugStore.Exception.ExceptionMessages;
using FluentValidation;

namespace BugStore.Application.Customer.Validators
{
    public class CustomerValidator : AbstractValidator<RequestCustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ResourceExceptionMessage.NAME_EMPTY);
            RuleFor(x => x.Email).NotEmpty().WithMessage(ResourceExceptionMessage.EMAIL_EMPTY);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(ResourceExceptionMessage.EMAIL_INVALID);
            RuleFor(x => x.Phone).NotEmpty().WithMessage(ResourceExceptionMessage.PHONE_EMPTY);
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .LessThan(DateTime.Now)
                .WithMessage(ResourceExceptionMessage.BIRTHDATE_INVALID);
        }
    }
}
