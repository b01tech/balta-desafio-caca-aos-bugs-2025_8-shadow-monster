using Bogus;
using BugStore.Domain.Entities;

namespace BugStore.TestUtilities.Builders
{
    public static class ProductBuilder
    {
        private static readonly Faker _faker = new("pt_BR");

        public static Product Build(string? title =null, string? description = null, string? slug = null, decimal? price = null)
        {
            title ??= _faker.Commerce.ProductName();
            description ??= _faker.Lorem.Paragraph();
            slug ??= _faker.Internet.DomainWord().ToLowerInvariant();
            price ??= _faker.Random.Decimal(1, 1000);

            return new Product(title, description, slug, price.Value);
        }
    }
}
