using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Shuttle.Core.Mediator;

public class MediatorBuilder(IServiceCollection services)
{
    private static readonly Type ParticipantType = typeof(IParticipant<>);

    private readonly Dictionary<Type, List<ParticipantDelegate>> _delegates = new();

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);

    public MediatorBuilder AddParticipant<TParticipant>()
    {
        AddParticipant(typeof(TParticipant));

        return this;
    }

    public MediatorBuilder AddParticipant(Type participantType, Func<Type, ServiceLifetime>? getServiceLifetime = null)
    {
        getServiceLifetime ??= _ => ServiceLifetime.Scoped;
        var isParticipantType = false;

        if (participantType.IsCastableTo(ParticipantType))
        {
            var participantInterface = participantType.GetInterface(ParticipantType.Name);

            if (participantInterface == null)
            {
                throw new InvalidOperationException(string.Format(Resources.InvalidParticipantTypeException, participantType.Name));
            }

            var genericType = ParticipantType.MakeGenericType(participantInterface.GetGenericArguments().First());

            Services.Add(new(genericType, participantType, getServiceLifetime(genericType)));

            isParticipantType = true;
        }

        if (!isParticipantType)
        {
            throw new InvalidOperationException(string.Format(Resources.InvalidParticipantTypeException, participantType.Name));
        }

        return this;
    }

    public MediatorBuilder AddParticipant(Delegate handler)
    {
        if (!typeof(Task).IsAssignableFrom(Guard.AgainstNull(handler).Method.ReturnType))
        {
            throw new ApplicationException(Resources.AsyncDelegateRequiredException);
        }

        var parameters = handler.Method.GetParameters();

        if (parameters.Length < 1)
        {
            throw new ApplicationException(Resources.ParticipantTypeException);
        }

        var messageType = parameters[0].ParameterType;

        _delegates.TryAdd(messageType, new());
        _delegates[messageType].Add(new(handler, handler.Method.GetParameters().Select(item => item.ParameterType)));

        return this;
    }

    public MediatorBuilder AddParticipant(object participant)
    {
        Guard.AgainstNull(participant);

        var participantType = participant.GetType();

        if (!participantType.IsCastableTo(ParticipantType))
        {
            throw new InvalidOperationException(string.Format(Resources.InvalidParticipantTypeException, participantType.Name));
        }

        var participantInterface = participantType.GetInterface(ParticipantType.Name);

        if (participantInterface == null)
        {
            throw new InvalidOperationException(string.Format(Resources.InvalidParticipantTypeException, participantType.Name));
        }

        Services.AddSingleton(ParticipantType.MakeGenericType(participantInterface.GetGenericArguments().First()), participant);

        return this;
    }

    public MediatorBuilder AddParticipants(Assembly assembly)
    {
        Guard.AgainstNull(assembly);

        foreach (var type in assembly.GetTypesCastableToAsync(ParticipantType).GetAwaiter().GetResult())
        {
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (@interface.Name != ParticipantType.Name)
                {
                    continue;
                }

                Services.TryAddScoped(ParticipantType.MakeGenericType(@interface.GetGenericArguments().First()), type);
            }
        }

        return this;
    }

    public MediatorBuilder AddParticipants()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            AddParticipants(assembly);
        }

        return this;
    }

    public IDictionary<Type, List<ParticipantDelegate>> GetDelegates()
    {
        return new ReadOnlyDictionary<Type, List<ParticipantDelegate>>(_delegates);
    }
}