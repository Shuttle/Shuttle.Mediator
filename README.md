# Shuttle.Core.Mediator

The Shuttle.Core.Mediator package provides a [mediator pattern](https://en.wikipedia.org/wiki/Mediator_pattern) implementation.

## Installation

```bash
dotnet add package Shuttle.Core.Mediator
```

## Configuration

In order to get all the relevant bits working you would need to register the `IMediator` dependency along with all the relevant `IParticipant` dependencies.

You can register the mediator using `IServiceCollection`:

```csharp
services.AddMediator(builder =>
{
    builder.AddParticipants(assembly);
    builder.AddParticipant<Participant>();
    builder.AddParticipant(participantType);
    builder.AddParticipant<Message>(participant);
    builder.AddParticipant(async (T message, CancellationToken cancellationToken) =>
    {
        await Task.CompletedTask.ConfigureAwait(false);
    });
});
```

## Usage

```csharp
// Define a message
public class UserCreated
{
    public string UserName { get; set; }
}

// Create participants
public class EmailNotificationParticipant : IParticipant<UserCreated>
{
    public Task ProcessMessageAsync(UserCreated message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Sending welcome email to {message.UserName}");
        return Task.CompletedTask;
    }
}

public class AuditLogParticipant : IParticipant<UserCreated>
{
    public Task ProcessMessageAsync(UserCreated message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Auditing user creation: {message.UserName}");
        return Task.CompletedTask;
    }
}

// Send the message
await mediator.SendAsync(new UserCreated { UserName = "john.doe" });
```

## IMediator

The core interface is the `IMediator` interface and the default implementation provided is the `Mediator` class.

Participants types are instantiated from the `IServiceProvider` instance. This means that it depends on how you register the type as to the behavior.

```csharp
Task SendAsync(object message, CancellationToken cancellationToken = default);
```

The `SendAsync` method will find all participants that implement the `IParticipant<T>` with the type `T` of the message type that you are sending.

## Participant implementations

```csharp
public interface IParticipant<in T>
{
    Task ProcessMessageAsync(T message, CancellationToken cancellationToken = default);
}
```

A participant would handle the message that is sent using the mediator.  There may be any number of participants that process the message. 

## Design philosophy

There are no *request/response* semantics and the design philosophy here is that the message encapsulates the state that is passed along in a *pipes & filters* approach.

However, you may wish to make use of one of the existing utility classes:-

### RequestMessage\<TRequest\>

The only expectation from a `RequestMessage<TRequest>` instance is either a success or failure (along with the failure message).

### RequestResponseMessage\<TRequest, TResponse\>

The `RequestResponseMessage<TRequest, TResponse>` takes an initial `TRequest` object and after the mediator processing would expect that there be a `TResponse` provided using the `.WithResponse(TResponse)` method.  The same success/failure mechanism used in the `RequestMessage<TRequest>` class is also available on this class.

## Considerations

If you have a rather predictable sequential workflow and you require something with faster execution then you may wish to consider the [Shuttle.Core.Pipelines](http://shuttle.github.io/shuttle-core/shuttle-core-pipelines) package.  

Performing a benchmark for your use-case would be able to indicate the more suitable option.
