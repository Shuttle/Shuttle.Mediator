using Shuttle.Extensions.Options;

namespace Shuttle.Mediator;

public class MediatorOptions
{
    public AsyncEvent<SendEventArgs> Sending { get; set; } = new();
    public AsyncEvent<SendEventArgs> Sent { get; set; } = new();
}