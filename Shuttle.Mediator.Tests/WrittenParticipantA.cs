using Shuttle.Contract;

namespace Shuttle.Mediator.Tests;

public class WrittenParticipantA : AbstractParticipant, IParticipant<MessageWritten>
{
    private readonly Guid _id = Guid.NewGuid();

    public async Task HandleAsync(MessageWritten message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($@"[event-{_id}] : text = '{Guard.AgainstNull(message).Text}'");

        await CallAsync();
    }
}