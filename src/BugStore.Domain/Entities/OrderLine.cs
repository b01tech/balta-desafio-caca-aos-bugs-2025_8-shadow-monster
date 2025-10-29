using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;

namespace BugStore.Domain.Entities;
public class OrderLine
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }

    public int Quantity { get; private set; }
    public decimal Total { get; private set; } 

    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    internal OrderLine(Guid orderId, Guid productId, int quantity, decimal price)
    {
        ValidateQuantity(quantity);
        Id = Guid.CreateVersion7();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        CalculateTotal(price);
    }

    protected OrderLine() { }

    public void UpdateQuantity(int quantity, decimal price)
    {
        ValidateQuantity(quantity);
        Quantity = quantity;
        CalculateTotal(price);
    }

    private void CalculateTotal(decimal price)
    {
        Total = price * Quantity;
    }

    private void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new OnValidationException(ResourceExceptionMessage.QUANTITY_INVALID);
    }
}
