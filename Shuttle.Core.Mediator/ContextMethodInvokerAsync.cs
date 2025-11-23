using System.Reflection;
using System.Reflection.Emit;

namespace Shuttle.Core.Mediator;

internal class ContextMethodInvokerAsync
{
    private static readonly Type ParticipantContextType = typeof(ParticipantContext<>);

    private readonly InvokeHandler _invoker;

    public ContextMethodInvokerAsync(MethodInfo methodInfo)
    {
        var dynamicMethod = new DynamicMethod(string.Empty,
            typeof(Task), [typeof(object), typeof(object)],
            ParticipantContextType.Module);

        var il = dynamicMethod.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);

        il.EmitCall(OpCodes.Callvirt, methodInfo, null);
        il.Emit(OpCodes.Ret);

        _invoker = (InvokeHandler)dynamicMethod.CreateDelegate(typeof(InvokeHandler));
    }

    public async Task Invoke(object participant, object participantContext)
    {
        await _invoker.Invoke(participant, participantContext);
    }

    private delegate Task InvokeHandler(object handler, object handlerContext);
}