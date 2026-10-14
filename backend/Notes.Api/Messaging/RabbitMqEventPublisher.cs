using System.Text.Json;
using RabbitMQ.Client;

namespace Notes.Api.Messaging;

public sealed class RabbitMqEventPublisher(IConnection connection) : IEventPublisher
{
    public const string Exchange = "notes.events";

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        await channel.BasicPublishAsync(Exchange, typeof(T).Name, body, ct);
    }
}
