namespace Shuttle.Core.Mediator;

public interface IMediator
{
    Task SendAsync(object message, CancellationToken cancellationToken = default);
}