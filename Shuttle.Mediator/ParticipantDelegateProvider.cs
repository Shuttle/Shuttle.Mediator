namespace Shuttle.Mediator;

public class ParticipantDelegateProvider(IDictionary<Type, List<ParticipantDelegate>> participantDelegates)
    : IParticipantDelegateProvider
{
    public IDictionary<Type, List<ParticipantDelegate>> Delegates { get; } = participantDelegates;
}