using Bogus;
using BugStore.Domain.Entities;

namespace BugStore.TestUtilities.Builders
{
    public static class OrderBuilder
    {
        private static readonly Faker _faker = new("pt_BR");

        public static Order Build(Guid? customerId = null)
        {
            var order = new Order(customerId ?? Guid.CreateVersion7());
            return order;
        }

        public static Order BuildWithLines(Guid? customerId = null, int lineCount = 2)
        {
            var order = Build(customerId);

            for (int i = 0; i < lineCount; i++)
            {
                var productId = Guid.CreateVersion7();
                var quantity = _faker.Random.Int(1, 10);
                var price = _faker.Random.Decimal(10, 1000);

                order.AddLine(productId, quantity, price);
            }

            return order;
        }

        public static Order BuildWithSpecificLine(
            Guid? customerId = null,
            Guid? productId = null,
            int quantity = 1,
            decimal price = 100m
        )
        {
            var order = Build(customerId);
            order.AddLine(productId ?? Guid.CreateVersion7(), quantity, price);
            return order;
        }
    }
}
