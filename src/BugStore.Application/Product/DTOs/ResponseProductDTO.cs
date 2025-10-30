namespace BugStore.Application.Product.DTOs
{
    public record ResponseProductDetailedDTO(
        Guid Id,
        string Title,
        string Description,
        string Slug,
        decimal Price
    );

    public record ResponseProductDTO(Guid Id, string Title, decimal Price);

    public record ResponseListProductDTO(
        long TotalItems,
        int Page,
        int TotalPages,
        IList<ResponseProductDTO> Products
    );
}
