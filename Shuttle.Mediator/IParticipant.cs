namespace Shuttle.Mediator;

public interface IParticipant<in T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}