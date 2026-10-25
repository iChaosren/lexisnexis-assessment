namespace Assessment.Api.Dto;

public sealed class CustomerResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;
}
