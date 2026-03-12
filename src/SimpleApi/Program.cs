

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => 
{
    var htmlPath = Path.Combine(AppContext.BaseDirectory, "index.html");
    var htmlContent = File.ReadAllText(htmlPath);
    return Results.Content(htmlContent, "text/html");
});

app.MapControllers();

app.Run();
