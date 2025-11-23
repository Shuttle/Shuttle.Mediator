using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class WriteParticipant : AbstractParticipant, IParticipant<WriteMessage>
{
    public async Task ProcessMessageAsync(IParticipantContext<WriteMessage> context)
    {
        Guard.AgainstNull(context);

        Console.WriteLine($@"[command] : text = '{context.Message.Text}'");

        await CallAsync();
    }
}