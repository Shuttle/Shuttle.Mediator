using System.Reflection.Emit;

namespace Shuttle.Core.Mediator;

internal class ContextConstructorInvoker
{
    private static readonly Type ParticipantContextType = typeof(ParticipantContext<>);

    private readonly ConstructorInvokeHandler _constructorInvoker;

    public ContextConstructorInvoker(Type messageType)
    {
        var dynamicMethod = new DynamicMethod(string.Empty, typeof(object),
        [
            typeof(object),
            typeof(CancellationToken)
        ], ParticipantContextType.Module);

        var il = dynamicMethod.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);

        var contextType = ParticipantContextType.MakeGenericType(messageType);
        var constructorInfo = contextType.GetConstructor([
            messageType,
            typeof(CancellationToken)
        ]);

        if (constructorInfo == null)
        {
            throw new InvalidOperationException(string.Format(Resources.ContextConstructorException, contextType.FullName));
        }

        il.Emit(OpCodes.Newobj, constructorInfo);
        il.Emit(OpCodes.Ret);

        _constructorInvoker =
            (ConstructorInvokeHandler)dynamicMethod.CreateDelegate(typeof(ConstructorInvokeHandler));
    }

    public object CreateParticipantContext(object message, CancellationToken cancellationToken)
    {
        return _constructorInvoker(message, cancellationToken);
    }

    private delegate object ConstructorInvokeHandler(object message, CancellationToken cancellationToken);
}