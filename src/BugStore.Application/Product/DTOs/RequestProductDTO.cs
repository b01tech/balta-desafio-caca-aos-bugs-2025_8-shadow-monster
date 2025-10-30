namespace BugStore.Application.Product.DTOs
{
    public record RequestProductDTO(string Title, string Description, string Slug, decimal Price);
}
