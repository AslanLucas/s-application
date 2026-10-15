using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Notes.Api.Events;
using Notes.Api.Messaging;
using RabbitMQ.Client;


namespace Notes.IntegrationTests;

public class NotesApiTests(NotesApiFactory factory) : IClassFixture<NotesApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private record NoteResponse(Guid Id, string Title, string Content);

    [Fact]
    public async Task Post_ThenGet_ReturnsCreatedNote()
    {
        var response = await _client.PostAsJsonAsync("/api/notes",
            new { title = "Integration", content = "Test" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<NoteResponse>();

        var fetched = await _client.GetFromJsonAsync<NoteResponse>($"/api/notes/{created!.Id}");
        Assert.Equal("Integration", fetched!.Title);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/notes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingNote_RemovesIt()
    {
        var created = await (await _client.PostAsJsonAsync("/api/notes",
            new { title = "Weg damit", content = "x" })).Content.ReadFromJsonAsync<NoteResponse>();

        var delete = await _client.DeleteAsync($"/api/notes/{created!.Id}");
        var get = await _client.GetAsync($"/api/notes/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Post_PublishesNoteCreatedEvent()
    {
        var connectionFactory = new ConnectionFactory { Uri = new Uri(factory.RabbitMqConnectionString) };
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(RabbitMqEventPublisher.Exchange, ExchangeType.Topic, durable: true);
        var queue = await channel.QueueDeclareAsync();
        await channel.QueueBindAsync(queue.QueueName, RabbitMqEventPublisher.Exchange, nameof(NoteCreated));

        await _client.PostAsJsonAsync("/api/notes", new { title = "Event", content = "x" });

        BasicGetResult? message = null;
        for (var i = 0; i < 20 && message is null; i++)
        {
            message = await channel.BasicGetAsync(queue.QueueName, autoAck: true);
            if (message is null) await Task.Delay(250);
        }

        Assert.NotNull(message);
        var evt = JsonSerializer.Deserialize<NoteCreated>(message.Body.Span);
        Assert.Equal("Event", evt!.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Post_WithEmptyTitle_ReturnsBadRequest(string? title)
    {
        var response = await _client.PostAsJsonAsync("/api/notes", new { title, content = "x" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


}
