
using System.ComponentModel;
using Assessment.Api.Dto;

public sealed class Order : IMapToDto<OrderResponse>
{
    public Order()
    {
    }

    public Order(CreateOrderRequest dto)
    {
        this.Id = Guid.NewGuid(dto.ExternalReference);
        this.ExternalReference = dto.ExternalReference;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
        this.Currency = dto.Currency;
        this.Lines = dto.Lines.Select((l) => new LineItem(l)).ToList();
    }
    
    public Guid Id { get; set; }
    
    public string ExternalReference { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public Customer Customer { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public Currency Currency { get; set; } = Currency.ZAR;
    
    public List<LineItem> Lines { get; set; } = new();

    public decimal SubTotal => Lines.Sum(line => line.LineTotal);

    public decimal Tax => SubTotal * 0.15m;
    
    public decimal Total => SubTotal + Tax;

    public Order UpdateStatus(ChangeOrderStatusRequest dto)
    {
        //if (this.Status is OrderStatus.Cancelled or OrderStatus.Fulfilled)
        //    throw new ArgumentException("Cannot update fullfilled/cancelled orders");
        
        //if (dto.NewStatus == OrderStatus.Pending)
        //    throw new ArgumentException("Cannot update orders to pending");

        var clone = (Order)this.MemberwiseClone();
        clone.Status = dto.NewStatus;
        clone.UpdatedAt = DateTime.UtcNow;
        return clone;
    }
    
    public OrderResponse ToDto()
    {
        return new OrderResponse()
        {
            Id = this.Id,
            ExternalReference = this.ExternalReference,
            CustomerId = this.CustomerId,
            Customer = this.Customer.ToDto(),
            CreatedAt = this.CreatedAt,
            UpdatedAt = this.UpdatedAt,
            Status = this.Status,
            Currency = this.Currency,
            Lines = this.Lines.Select(line => line.ToDto()).ToList(),
            SubTotal = this.SubTotal,
            Tax = this.Tax,
            Total = this.Total
        };
    }
}
