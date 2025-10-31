namespace BugStore.Application.Reports.DTOs
{
    public record RequestRevenueByPeriodDTO(
        DateTime StartDate,
        DateTime EndDate
    );
}
