namespace BugStore.Application.Reports.DTOs
{
    public record ResponseRevenueByCustomerDTO(
        Guid CustomerId,
        string CustomerName,
        long TotalOrders,
        decimal TotalSpent);
}
