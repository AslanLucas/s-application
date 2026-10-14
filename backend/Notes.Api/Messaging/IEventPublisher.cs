namespace Notes.Api.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default);
}
