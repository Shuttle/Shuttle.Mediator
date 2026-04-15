namespace Shuttle.Mediator;

public interface IParticipantDelegateProvider
{
    IDictionary<Type, List<ParticipantDelegate>> Delegates { get; }
}