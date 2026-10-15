using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Notes.IntegrationTests;

public class BrokerUnavailableTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Post_WithEmptyTitle_ReturnsBadRequest_EvenWhenBrokerIsDown()
    {
        using var app = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Notes",
                "Host=localhost;Port=1;Database=x;Username=x;Password=x");
            builder.UseSetting("ConnectionStrings:RabbitMq", "amqp://guest:guest@localhost:1/");
        });
        var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/notes", new { title = "", content = "x" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
