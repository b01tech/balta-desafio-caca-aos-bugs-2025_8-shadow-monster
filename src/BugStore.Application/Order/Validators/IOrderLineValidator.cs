using BugStore.Application.Order.DTOs;
using FluentValidation.Results;

namespace BugStore.Application.Order.Validators
{
    public interface IOrderLineValidator
    {
        Task<ValidationResult> ValidateAsync(RequestOrderLineDTO request, CancellationToken cancellationToken = default);
    }
}