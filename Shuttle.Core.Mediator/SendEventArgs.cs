using Shuttle.Contract;

namespace Shuttle.Mediator;

public class SendEventArgs(object message) : EventArgs
{
    public object Message { get; } = Guard.AgainstNull(message);
}