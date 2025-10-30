namespace BugStore.Application.Customer.DTOs;
public record ResponseCreatedCustomerDTO(Guid Id, string Name, string Email, DateTime CreatedAt);
