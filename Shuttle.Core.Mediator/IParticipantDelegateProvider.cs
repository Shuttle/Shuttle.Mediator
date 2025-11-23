namespace Shuttle.Core.Mediator;

public interface IParticipantDelegateProvider
{
    IDictionary<Type, List<ParticipantDelegate>> Delegates { get; }
}