using BugStore.Application.Order.DTOs;

namespace BugStore.TestUtilities.Builders;

public static class OrderDTOBuilder
{
    public static RequestCreateOrderDTO BuildCreateRequest(Guid? customerId = null)
    {
        return new RequestCreateOrderDTO(customerId ?? Guid.CreateVersion7());
    }

    public static RequestOrderDTO BuildRequest(
        Guid? orderId = null,
        List<RequestOrderLineDTO>? lines = null
    )
    {
        return new RequestOrderDTO(
            orderId ?? Guid.CreateVersion7(),
            lines ?? new List<RequestOrderLineDTO> { BuildLineRequest() }
        );
    }

    public static RequestOrderLineDTO BuildLineRequest(
        Guid? productId = null,
        int quantity = 2,
        decimal price = 50.00m
    )
    {
        return new RequestOrderLineDTO(productId ?? Guid.CreateVersion7(), quantity, price);
    }

    public static RequestRemoveProductDTO BuildRemoveRequest(Guid? productId = null)
    {
        return new RequestRemoveProductDTO(productId ?? Guid.CreateVersion7());
    }

    public static ResponseOrderDetailedDTO BuildDetailedResponse(
        Guid? id = null,
        Guid? customerId = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        List<OrderLineDTO>? lines = null,
        decimal total = 100.00m
    )
    {
        return new ResponseOrderDetailedDTO(
            id ?? Guid.CreateVersion7(),
            customerId ?? Guid.CreateVersion7(),
            createdAt ?? DateTime.UtcNow,
            updatedAt,
            lines ?? new List<OrderLineDTO> { BuildLineResponse() },
            total
        );
    }

    public static ResponseOrderSummaryDTO BuildSummaryResponse(
        Guid? id = null,
        Guid? customerId = null,
        DateTime? createdAt = null,
        decimal total = 100.00m
    )
    {
        return new ResponseOrderSummaryDTO(
            id ?? Guid.CreateVersion7(),
            customerId ?? Guid.CreateVersion7(),
            createdAt ?? DateTime.UtcNow,
            total
        );
    }

    public static OrderLineDTO BuildLineResponse(
        Guid? productId = null,
        int quantity = 2,
        decimal price = 50.00m,
        decimal? total = null
    )
    {
        return new OrderLineDTO(
            productId ?? Guid.CreateVersion7(),
            quantity,
            price,
            total ?? (quantity * price)
        );
    }
}
