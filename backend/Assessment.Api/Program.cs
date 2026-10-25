var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/api/health");

app.Run();

// Allows the test project to host the real API pipeline in memory.
public partial class Program;
