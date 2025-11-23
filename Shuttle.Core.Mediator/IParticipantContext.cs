namespace Shuttle.Core.Mediator;

public interface IParticipantContext<out TRequest>
{
    CancellationToken CancellationToken { get; }
    TRequest Message { get; }
}