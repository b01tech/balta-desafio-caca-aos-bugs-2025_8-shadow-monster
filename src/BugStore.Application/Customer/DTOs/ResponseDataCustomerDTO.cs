namespace BugStore.Application.Customer.DTOs;
public record ResponseDataCustomerDTO(Guid Id, string Name, string Email, string Phone, DateTime BirthDate);

public record ResponseListCustomerDTO(long TotalCustomers, int Page,int TotalPages, IList<ResponseDataCustomerDTO> Customers);
