namespace BugStore.Application.Order.DTOs
{
    public record ResponseOrderDetailedDTO(
        Guid Id,
        Guid CustomerId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<OrderLineDTO> Lines,
        decimal Total
    );

    public record ResponseOrderSummaryDTO(
        Guid Id,
        Guid CustomerId,
        DateTime CreatedAt,
        decimal Total
    );

    public record OrderLineDTO(Guid ProductId, int Quantity, decimal Price, decimal Total);

    public record ResponseListOrderDTO(
        long TotalItems,
        int Page,
        int TotalPages,
        IList<ResponseOrderSummaryDTO> Orders
    );
}
