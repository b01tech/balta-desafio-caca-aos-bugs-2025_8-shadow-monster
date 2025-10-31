using Bogus;
using BugStore.Domain.Entities;

namespace BugStore.TestUtilities.Builders;

public static class CustomerBuilder
{
    private static readonly Faker _faker = new("pt_BR");

    public static Customer Build(
        string? name = null,
        string? email = null,
        string? phone = null,
        DateTime? birthDate = null
    )
    {
        return new Customer(
            name: name ?? _faker.Person.FullName,
            email: email ?? _faker.Person.Email,
            phone: phone ?? _faker.Phone.PhoneNumber("(##) #####-####"),
            birthDate: birthDate ?? _faker.Person.DateOfBirth.Date
        );
    }
}
