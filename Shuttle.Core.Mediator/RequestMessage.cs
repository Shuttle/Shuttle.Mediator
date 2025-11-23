using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator;

public class RequestMessage<TRequest>
{
    public RequestMessage(TRequest request)
    {
        if (!typeof(TRequest).IsValueType)
        {
            Guard.AgainstNull(request);
        }

        Request = request;
    }

    public string Message { get; private set; } = string.Empty;

    public bool Ok => string.IsNullOrWhiteSpace(Message);

    public TRequest Request { get; }

    public RequestMessage<TRequest> Failed(string message)
    {
        Guard.AgainstEmpty(message);

        Message = message;

        return this;
    }
}