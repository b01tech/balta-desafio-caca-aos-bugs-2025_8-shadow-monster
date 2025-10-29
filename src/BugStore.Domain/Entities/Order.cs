using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;

namespace BugStore.Domain.Entities;
public class Order
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Customer Customer { get; private set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public List<OrderLine> Lines { get; private set; } = new();

    public decimal Total => Lines.Sum(line => line.Total);

    protected Order() { }

    public Order(Guid customerId)
    {
        Id = Guid.CreateVersion7();
        CustomerId = customerId;
    }

    public void AddLine(Guid productId, int quantity, decimal price)
    {
        if (Lines.Any(l => l.ProductId.Equals(productId)))
            throw new OnInvalidOperationException(ResourceExceptionMessage.PRODUCT_DUPLICATED);

        var orderLine = new OrderLine(Id, productId, quantity, price);
        Lines.Add(orderLine);
        UpdateTimestamp();
    }

    public void RemoveLine(OrderLine orderLine)
    {
        Lines.Remove(orderLine);
        UpdateTimestamp();
    }

    private void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
