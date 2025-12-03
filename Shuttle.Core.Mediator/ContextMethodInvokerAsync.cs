using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Shuttle.Core.Mediator;

internal class ParticipantInvokerAsync
{
    private readonly ProcessMessageAsync _invoker;

    public ParticipantInvokerAsync(MethodInfo methodInfo)
    {
        var dynamicMethod = new DynamicMethod(string.Empty, typeof(Task), [typeof(object), typeof(object), typeof(CancellationToken)], GetType().Module);

        var il = dynamicMethod.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);

        il.EmitCall(OpCodes.Callvirt, methodInfo, null);
        il.Emit(OpCodes.Ret);

        _invoker = (ProcessMessageAsync)dynamicMethod.CreateDelegate(typeof(ProcessMessageAsync));
    }

    public async Task Invoke(object participant, object message, CancellationToken cancellationToken)
    {
        await _invoker.Invoke(participant, message, cancellationToken);
    }

    private delegate Task ProcessMessageAsync(object handler, object message, CancellationToken cancellationToken);
}