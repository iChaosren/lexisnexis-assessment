namespace Assessment.Api.Dto;

public sealed class CustomerResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }
}
