namespace Shuttle.Core.Mediator;

public interface IParticipant<in T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}