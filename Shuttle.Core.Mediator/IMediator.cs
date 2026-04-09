namespace Shuttle.Mediator;

public interface IMediator
{
    Task SendAsync(object message, CancellationToken cancellationToken = default);
}