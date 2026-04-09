using Microsoft.Extensions.DependencyInjection;

namespace Shuttle.Mediator;

public class ParticipantDelegate(Delegate handler, IEnumerable<Type> parameterTypes)
{
    private static readonly Type CancellationTokenType = typeof(CancellationToken);
    public Delegate Handler { get; } = handler;

    public object[] GetParameters(IServiceProvider serviceProvider, object message, CancellationToken cancellationToken)
    {
        var messageType = message.GetType();

        return parameterTypes
            .Select(parameterType =>
                parameterType == CancellationTokenType
                    ? cancellationToken
                    : parameterType == messageType
                        ? message
                        : serviceProvider.GetRequiredService(parameterType)
            ).ToArray();
    }
}