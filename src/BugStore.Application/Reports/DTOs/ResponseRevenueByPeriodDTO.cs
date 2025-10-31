namespace BugStore.Application.Reports.DTOs
{
    public record ResponseRevenueByPeriodDTO(
        DateTime StartDate,
        DateTime EndDate,
        long TotalOrders,
        decimal TotalRevenue);
}
