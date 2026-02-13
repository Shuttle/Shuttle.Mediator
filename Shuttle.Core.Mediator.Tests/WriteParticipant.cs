using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class WriteParticipant : AbstractParticipant, IParticipant<WriteMessage>
{
    public async Task HandleAsync(WriteMessage message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($@"[command] : text = '{Guard.AgainstNull(message).Text}'");

        await CallAsync();
    }
}