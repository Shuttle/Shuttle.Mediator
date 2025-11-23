namespace Shuttle.Core.Mediator.Tests;

public abstract class AbstractParticipant
{
    public int CallCount { get; private set; }

    public Guid Id { get; } = Guid.NewGuid();
    public DateTime WhenCalled { get; private set; }

    public void Call()
    {
        CallCount++;
        WhenCalled = DateTime.Now;
    }

    public async Task CallAsync()
    {
        Call();

        await Task.CompletedTask.ConfigureAwait(false);
    }
}