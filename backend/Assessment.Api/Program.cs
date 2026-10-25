var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.IncludeFields = true);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IOrderDb, OrderDb>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.MapHealthChecks("/api/health");
app.MapControllers();

app.Run();

// Allows the test project to host the real API pipeline in memory.
public partial class Program;
