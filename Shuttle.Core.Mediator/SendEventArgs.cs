using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator;

public class SendEventArgs(object message) : EventArgs
{
    public object Message { get; } = Guard.AgainstNull(message);
}