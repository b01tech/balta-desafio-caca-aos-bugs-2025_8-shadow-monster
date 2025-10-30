namespace BugStore.Application.Order.DTOs
{
    public record RequestOrderLineDTO(Guid ProductId, int Quantity, decimal Price);
}
