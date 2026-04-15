using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Mediator.Tests;

[TestFixture]
public class MediatorFixture
{
    [Test]
    public async Task Should_be_able_to_send_a_message_to_a_single_participant_delegate_async()
    {
        var provider = new ServiceCollection()
            .AddMediator().AddParticipant<WriteParticipant>()
            .Services
            .BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new WriteMessage { Text = "hello world!" });

        Assert.That(((AbstractParticipant)provider.GetRequiredService<IParticipant<WriteMessage>>()).CallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Should_be_able_to_send_a_message_to_a_single_participant_async()
    {
        var callCount = 0;

        var provider = new ServiceCollection()
            .AddMediator()
            .AddParticipant(async (WriteMessage _) =>
            {
                callCount++;

                await Task.CompletedTask.ConfigureAwait(false);
            })
            .Services
            .BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new WriteMessage { Text = "hello world!" });

        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Should_be_able_send_a_message_to_multiple_participants_async()
    {
        var provider = new ServiceCollection()
            .AddMediator(options =>
            {
                options.Sending += (args, _) =>
                {
                    Console.WriteLine($@"[sending] : text = '{((MessageWritten)args.Message).Text}'");

                    return Task.CompletedTask;
                };

                options.Sent += (args, _) =>
                {
                    Console.WriteLine($@"[sent] : text = '{((MessageWritten)args.Message).Text}'");

                    return Task.CompletedTask;
                };
            })
            .AddParticipant<WrittenParticipantA>()
            .AddParticipant<WrittenParticipantB>()
            .Services
            .BuildServiceProvider();

        var mediator = new Mediator(provider.GetRequiredService<IOptions<MediatorOptions>>(), provider, new ParticipantDelegateProvider(new Dictionary<Type, List<ParticipantDelegate>>()));

        await mediator.SendAsync(new MessageWritten { Text = "hello participants!" });

        foreach (var participant in provider.GetServices<IParticipant<MessageWritten>>())
        {
            Assert.That(((AbstractParticipant)participant).CallCount, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_pipeline_processing_async()
    {
        var registerA = new RegisterParticipant();
        var registerB = new RegisterParticipant();

        var provider = new ServiceCollection().AddMediator(options =>
            {
                options.Sending += (args, _) =>
                {
                    Console.WriteLine($@"[sending] : messages = '{string.Join(" / ", ((RegisterMessage)args.Message).Messages)}'");

                    return Task.CompletedTask;
                };

                options.Sent += (args, _) =>
                {
                    Console.WriteLine($@"[sent] : messages = '{string.Join(" / ", ((RegisterMessage)args.Message).Messages)}'");

                    return Task.CompletedTask;
                };
            })
            .AddParticipant(registerA)
            .AddParticipant(registerB)
            .Services
            .BuildServiceProvider();

        var mediator = new Mediator(provider.GetRequiredService<IOptions<MediatorOptions>>(), provider, new ParticipantDelegateProvider(new Dictionary<Type, List<ParticipantDelegate>>()));
        var message = new RegisterMessage();

        await mediator.SendAsync(message);

        Assert.That(message.Messages.Count(), Is.EqualTo(2));

        Assert.That(registerA.CallCount, Is.EqualTo(1));
        Assert.That(registerB.CallCount, Is.EqualTo(1));

        Assert.That(registerB.WhenCalled, Is.GreaterThan(registerA.WhenCalled));

        foreach (var text in message.Messages)
        {
            Console.WriteLine(text);
        }
    }
}