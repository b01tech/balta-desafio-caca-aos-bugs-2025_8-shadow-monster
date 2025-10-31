using BugStore.Application.Order.DTOs;
using BugStore.Exception.ExceptionMessages;
using FluentValidation;

namespace BugStore.Application.Order.Validators
{
    public class OrderLineValidator : AbstractValidator<RequestOrderLineDTO>, IOrderLineValidator
    {
        public OrderLineValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(ResourceExceptionMessage.PRODUCT_ID_EMPTY);

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage(ResourceExceptionMessage.QUANTITY_EMPTY)
                .GreaterThan(0)
                .WithMessage(ResourceExceptionMessage.QUANTITY_MUST_BE_POSITIVE);

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(ResourceExceptionMessage.ORDER_LINE_PRICE_EMPTY)
                .GreaterThan(0)
                .WithMessage(ResourceExceptionMessage.ORDER_LINE_PRICE_INVALID);
        }
    }
}
