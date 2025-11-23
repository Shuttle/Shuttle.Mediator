using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.Tests;

public class MessageTracker : IMessageTracker
{
    private readonly List<object> _messagesReceived = new();

    public void Received(object message)
    {
        _messagesReceived.Add(Guard.AgainstNull(message));
    }

    public int MessageTypeCount<T>()
    {
        return _messagesReceived.Count(item => item.GetType() == typeof(T));
    }
}