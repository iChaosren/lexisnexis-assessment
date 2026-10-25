using Assessment.Api.Dto;

public sealed class Customer : IMapToDto<CustomerResponse>
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Email { get; set; }

    public CustomerResponse ToDto()
    {
        return new CustomerResponse()
        {
            Id = this.Id,
            Name = this.Name,
            Email = this.Email
        };
    }
}
