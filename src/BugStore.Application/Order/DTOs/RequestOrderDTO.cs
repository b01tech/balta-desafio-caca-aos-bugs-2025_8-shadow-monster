namespace BugStore.Application.Order.DTOs
{
    public record RequestCreateOrderDTO(Guid CustomerId);

    public record RequestOrderDTO(Guid OrderId, List<RequestOrderLineDTO> Lines);
}
