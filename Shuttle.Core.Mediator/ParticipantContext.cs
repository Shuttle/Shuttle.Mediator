using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator;

public class ParticipantContext<TRequest>(TRequest message, CancellationToken cancellationToken) : IParticipantContext<TRequest>
{
    public TRequest Message { get; } = Guard.AgainstNull(message);
    public CancellationToken CancellationToken { get; } = cancellationToken;
}