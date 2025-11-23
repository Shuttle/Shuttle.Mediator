using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Core.Mediator.Tests;

[TestFixture]
public class MediatorFixture
{
    [Test]
    public async Task Should_be_able_to_send_a_message_to_a_single_participant_delegate_async()
    {
        var services = new ServiceCollection();

        services.AddMediator(builder =>
        {
            builder.AddParticipant<WriteParticipant>();
        });

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new WriteMessage { Text = "hello world!" });

        Assert.That(((AbstractParticipant)provider.GetRequiredService<IParticipant<WriteMessage>>()).CallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Should_be_able_to_send_a_message_to_a_single_participant_async()
    {
        var services = new ServiceCollection();

        var callCount = 0;

        services.AddMediator(builder =>
        {
            builder.AddParticipant(async (IParticipantContext<WriteMessage> _) =>
            {
                callCount++;

                await Task.CompletedTask.ConfigureAwait(false);
            });
        });

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new WriteMessage { Text = "hello world!" });

        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Should_be_able_send_a_message_to_multiple_participants_async()
    {
        var services = new ServiceCollection();

        services.AddMediator(builder =>
        {
            builder.AddParticipant<WrittenParticipantA>();
            builder.AddParticipant<WrittenParticipantB>();

            builder.Options.Sending += (args, _) =>
            {
                Console.WriteLine($@"[sending] : text = '{((MessageWritten)args.Message).Text}'");

                return Task.CompletedTask;
            };

            builder.Options.Sent += (args, _) =>
            {
                Console.WriteLine($@"[sent] : text = '{((MessageWritten)args.Message).Text}'");

                return Task.CompletedTask;
            };
        });

        var provider = services.BuildServiceProvider();
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
        var services = new ServiceCollection();

        var registerA = new RegisterParticipant();
        var registerB = new RegisterParticipant();

        services.AddMediator(builder =>
        {
            builder.AddParticipant(registerA);
            builder.AddParticipant(registerB);

            builder.Options.Sending += (args, _) =>
            {
                Console.WriteLine($@"[sending] : messages = '{string.Join(" / ", ((RegisterMessage)args.Message).Messages)}'");

                return Task.CompletedTask;
            };

            builder.Options.Sent += (args, _) =>
            {
                Console.WriteLine($@"[sent] : messages = '{string.Join(" / ", ((RegisterMessage)args.Message).Messages)}'");

                return Task.CompletedTask;
            };
        });

        var provider = services.BuildServiceProvider();
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