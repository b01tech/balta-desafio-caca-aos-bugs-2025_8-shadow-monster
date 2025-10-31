using System.Text.RegularExpressions;
using BugStore.Application.Product.DTOs;
using BugStore.Exception.ExceptionMessages;
using FluentValidation;

namespace BugStore.Application.Product.Validators;

public class ProductValidator : AbstractValidator<RequestProductDTO>
{
    public ProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage(ResourceExceptionMessage.TITLE_EMPTY);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ResourceExceptionMessage.DESCRIPTION_EMPTY);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage(ResourceExceptionMessage.SLUG_EMPTY)
            .Must(BeValidSlug)
            .WithMessage(ResourceExceptionMessage.SLUG_INVALID);

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage(ResourceExceptionMessage.PRICE_EMPTY)
            .GreaterThan(0)
            .WithMessage(ResourceExceptionMessage.PRICE_INVALID);
    }

    private static bool BeValidSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        // Slug deve conter apenas letras minúsculas, números e hífens
        var regex = new Regex(@"^[a-z0-9-]+$");
        return regex.IsMatch(slug);
    }
}
