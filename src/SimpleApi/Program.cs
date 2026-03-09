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

app.MapGet("/health", () => Results.Ok("OK"));

app.MapGet("/api/info", () =>
{
    var environment = app.Environment.EnvironmentName;
    var studentName = Environment.GetEnvironmentVariable("STUDENT_NAME") ?? "Unknown Student";
    
    var response = new
    {
        student = studentName,
        environment = environment,
        serverTimeUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
    };
    
    return Results.Ok(response);
});

app.Run();
