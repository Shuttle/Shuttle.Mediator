using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class WrittenParticipantA : AbstractParticipant, IParticipant<MessageWritten>
{
    private readonly Guid _id = Guid.NewGuid();

    public async Task ProcessMessageAsync(MessageWritten message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($@"[event-{_id}] : text = '{Guard.AgainstNull(message).Text}'");

        await CallAsync();
    }
}