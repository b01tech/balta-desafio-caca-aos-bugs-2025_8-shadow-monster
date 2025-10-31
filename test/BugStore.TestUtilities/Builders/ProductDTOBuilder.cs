using BugStore.Application.Product.DTOs;

namespace BugStore.TestUtilities.Builders;

public static class ProductDTOBuilder
{
    public static RequestProductDTO BuildRequest(
        string title = "Sample Product",
        string description = "Sample product description",
        string slug = "sample-product",
        decimal price = 99.99m
    )
    {
        return new RequestProductDTO(title, description, slug, price);
    }

    public static RequestPriceDTO BuildPriceRequest(decimal price = 149.99m)
    {
        return new RequestPriceDTO(price);
    }

    public static ResponseProductDetailedDTO BuildDetailedResponse(
        Guid? id = null,
        string title = "Sample Product",
        string description = "Sample product description",
        string slug = "sample-product",
        decimal price = 99.99m
    )
    {
        return new ResponseProductDetailedDTO(
            id ?? Guid.CreateVersion7(),
            title,
            description,
            slug,
            price
        );
    }
}
