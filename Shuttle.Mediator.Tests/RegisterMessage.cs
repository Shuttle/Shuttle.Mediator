namespace Shuttle.Mediator.Tests;

public class RegisterMessage
{
    private readonly List<string> _messages = new();

    public IEnumerable<string> Messages => _messages.AsReadOnly();

    public int TouchCount { get; private set; }

    public void Touch(string message)
    {
        TouchCount++;

        _messages.Add(message);
    }
}