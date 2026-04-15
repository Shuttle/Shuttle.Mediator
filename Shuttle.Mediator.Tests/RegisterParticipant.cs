using Shuttle.Contract;

namespace Shuttle.Mediator.Tests;

public class RegisterParticipant : AbstractParticipant, IParticipant<RegisterMessage>
{
    public async Task HandleAsync(RegisterMessage message, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(message).Touch($"[proper] : {Id}");

        await CallAsync();
    }
}