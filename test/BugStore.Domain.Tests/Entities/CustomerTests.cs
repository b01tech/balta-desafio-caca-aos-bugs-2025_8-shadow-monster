using BugStore.Domain.Entities;
using BugStore.TestUtilities.Builders;

namespace BugStore.Domain.Tests.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void GivenValidData_WhenCreatingCustomer_ThenCustomerShouldBeCreated()
        {
            // Arrange
            var name = "ValidName";
            var email = "valid@email.com";
            var phone = "123-456-7890";
            var birthDate = DateTime.UtcNow.AddYears(-18);

            // Act
            var customer = new Customer(name, email, phone, birthDate);

            // Assert
            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.Equal(name, customer.Name);
            Assert.Equal(email, customer.Email);
            Assert.Equal(phone, customer.Phone);
            Assert.Equal(birthDate, customer.BirthDate);
        }

        [Fact]
        public void GivenValidData_WhenCreatingCustomer_ShouldGenerateUniqueIds()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var anotherCustomer = new Customer(customer.Name, customer.Email, customer.Phone, customer.BirthDate);

            // Act & Assert
            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.NotEqual(Guid.Empty, anotherCustomer.Id);
            Assert.NotEqual(customer.Id, anotherCustomer.Id);
        }

        [Fact]
        public void GivenValidData_WhenUpdateCustomer_ShouldUpdateAllProperties()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var originalId = customer.Id;
            var newName = "New Name";
            var newEmail = "new_email@email.com";
            var newPhone = "123-456-7890";
            var newBirthDate = DateTime.UtcNow.AddYears(-25);

            // Act
            customer.Update(newName, newEmail, newPhone, newBirthDate);

            // Assert
            Assert.Equal(originalId, customer.Id);
            Assert.Equal(newName, customer.Name);
            Assert.Equal(newEmail, customer.Email);
            Assert.Equal(newPhone, customer.Phone);
            Assert.Equal(newBirthDate, customer.BirthDate);
        }
    }
}
