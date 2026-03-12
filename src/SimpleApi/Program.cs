using SimpleApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

app.MapApiEndpoints();

app.Run();
