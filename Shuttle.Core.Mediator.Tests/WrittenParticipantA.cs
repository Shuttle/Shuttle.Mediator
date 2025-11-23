using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class WrittenParticipantA : AbstractParticipant, IParticipant<MessageWritten>
{
    private readonly Guid _id = Guid.NewGuid();

    public async Task ProcessMessageAsync(IParticipantContext<MessageWritten> context)
    {
        Guard.AgainstNull(context);

        Console.WriteLine($@"[event-{_id}] : text = '{context.Message.Text}'");

        await CallAsync();
    }
}