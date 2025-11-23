using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Reflection;

namespace Shuttle.Core.Mediator;

public class ParticipantDelegate(Delegate handler, IEnumerable<Type> parameterTypes)
{
    private static readonly Type ParticipantContextType = typeof(IParticipantContext<>);

    public Delegate Handler { get; } = handler;
    public bool HasParameters { get; } = parameterTypes.Any();

    public object[] GetParameters(IServiceProvider serviceProvider, object handlerContext)
    {
        return parameterTypes
            .Select(parameterType => !parameterType.IsCastableTo(ParticipantContextType)
                ? serviceProvider.GetRequiredService(parameterType)
                : handlerContext
            ).ToArray();
    }
}