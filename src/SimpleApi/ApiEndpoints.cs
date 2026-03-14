namespace SimpleApi;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => Results.Ok("200 OK"));

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
    }
}
