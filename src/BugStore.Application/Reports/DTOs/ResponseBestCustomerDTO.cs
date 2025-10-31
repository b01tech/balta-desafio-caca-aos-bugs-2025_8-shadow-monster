namespace BugStore.Application.Reports.DTOs
{
    public record ResponseBestCustomersListDTO(
        IList<ResponseBestCustomerDTO> Customers);
    public record ResponseBestCustomerDTO(
        Guid CustomerId,
        string CustomerName,
        long TotalOrders,
        decimal TotalSpent);
}
