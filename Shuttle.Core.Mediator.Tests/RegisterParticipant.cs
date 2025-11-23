using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class RegisterParticipant : AbstractParticipant, IParticipant<RegisterMessage>
{
    public async Task ProcessMessageAsync(IParticipantContext<RegisterMessage> context)
    {
        Guard.AgainstNull(context);

        context.Message.Touch($"[proper] : {Id}");

        await CallAsync();
    }
}