using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class MultipleParticipants(IMessageTracker messageTracker) :
    IParticipant<MultipleParticipantMessageA>,
    IParticipant<MultipleParticipantMessageB>
{
    private readonly IMessageTracker _messageTracker = Guard.AgainstNull(messageTracker);

    public async Task HandleAsync(MultipleParticipantMessageA message, CancellationToken cancellationToken = default)
    {
        _messageTracker.Received(message);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task HandleAsync(MultipleParticipantMessageB message, CancellationToken cancellationToken = default)
    {
        _messageTracker.Received(message);

        await Task.CompletedTask.ConfigureAwait(false);
    }
}