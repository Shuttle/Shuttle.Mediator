namespace Shuttle.Core.Mediator;

public interface IParticipant<in T>
{
    Task ProcessMessageAsync(T message, CancellationToken cancellationToken = default);
}