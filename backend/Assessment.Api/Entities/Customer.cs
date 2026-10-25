
public sealed class Customer : IMapToDto<CustomerResponse>
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }

    public CustomerResponse ToDto()
    {
        return new CustomerResponse()
        {
            // fill me out
        };
    }
}
