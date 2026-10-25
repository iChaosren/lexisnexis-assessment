
using System.ComponentModel.DataAnnotations;
using Assessment.Api.Dto;

public sealed class LineItem : IMapToDto<OrderLineResponse>
{
    public LineItem(CreateOrderLineRequest lineItem)
    {
        this.SKU = lineItem.SKU;
        this.Name = lineItem.Name;
        this.Quantity = lineItem.Quantity;
        this.UnitPrice = lineItem.UnitPrice;
    }
    
    // If it were a db entry:
    // public Guid OrderId { get; set; }
    // public Order Order { get; set; }
    
    public string SKU { get; set; }
    
    public string Name { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity;
    
    [Range(typeof(decimal), "0", DecimalExtensions.MaxValue)]
    public decimal UnitPrice;
    
    public decimal LineTotal => Quantity * UnitPrice;
    
    public OrderLineResponse ToDto()
    {
        return new OrderLineResponse()
        {
            SKU = this.SKU,
            Name = this.Name,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            LineTotal = this.LineTotal
        };
    }
}
