using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SimpleApi.Controllers;

namespace SimpleApi.Tests;

public class UnitTest1
{
    [Fact]
    public void Health_Should_Return_200_OK_String()
    {
        var environment = CreateEnvironment("Development");
        var controller = new SystemInfoController(environment);

        var result = controller.Health() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal("200 OK", result!.Value);
    }

    [Fact]
    public void Info_Should_Return_Environment_And_StudentName()
    {
        var environment = CreateEnvironment("Production");
        Environment.SetEnvironmentVariable("STUDENT_NAME", "Test Student");
        var controller = new SystemInfoController(environment);

        var result = controller.Info() as OkObjectResult;

        Assert.NotNull(result);
        Assert.NotNull(result!.Value);

        var json = JsonSerializer.Serialize(result.Value);
        Assert.Contains("\"student\":\"Test Student\"", json);
        Assert.Contains("\"environment\":\"Production\"", json);
    }

    private static IWebHostEnvironment CreateEnvironment(string environmentName)
    {
        return new HostingEnvironment
        {
            EnvironmentName = environmentName
        };
    }

    private class HostingEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
