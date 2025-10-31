using BugStore.Application.Customer.DTOs;

namespace BugStore.TestUtilities.Builders;

public static class CustomerDTOBuilder
{
    public static RequestCustomerDTO BuildRequest(
        string name = "John Doe",
        string email = "john.doe@example.com",
        string phone = "+1234567890",
        DateTime? birthDate = null
    )
    {
        return new RequestCustomerDTO(name, email, phone, birthDate ?? new DateTime(1990, 1, 1));
    }

    public static ResponseDataCustomerDTO BuildResponse(
        Guid? id = null,
        string name = "John Doe",
        string email = "john.doe@example.com",
        string phone = "+1234567890",
        DateTime? birthDate = null
    )
    {
        return new ResponseDataCustomerDTO(
            id ?? Guid.CreateVersion7(),
            name,
            email,
            phone,
            birthDate ?? new DateTime(1990, 1, 1)
        );
    }
}
