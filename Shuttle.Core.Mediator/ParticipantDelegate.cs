using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Mediator;

public class ParticipantDelegate(Delegate handler, IEnumerable<Type> parameterTypes)
{
    public Delegate Handler { get; } = handler;
    public bool HasParameters { get; } = parameterTypes.Any();

    public object[] GetParameters(IServiceProvider serviceProvider, object message)
    {
        var messageType = message.GetType();

        return parameterTypes
            .Select(parameterType => !parameterType.IsCastableTo(messageType)
                ? serviceProvider.GetRequiredService(parameterType)
                : message
            ).ToArray();
    }
}