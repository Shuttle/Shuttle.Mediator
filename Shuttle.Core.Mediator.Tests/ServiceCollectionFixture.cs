using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Shuttle.Core.Mediator.Tests;

[TestFixture]
public class ServiceCollectionFixture
{
    [Test]
    public void Should_be_able_to_handle_MessageWritten()
    {
        const int count = 100;

        var provider = new ServiceCollection()
            .AddMediator()
            .AddParticipantsFrom(GetType().Assembly)
            .Services
            .BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        var sw = new Stopwatch();

        sw.Start();

        for (var i = 0; i < count; i++)
        {
            mediator.SendAsync(new MessageWritten { Text = "hello participants!" });
        }

        sw.Stop();

        Console.WriteLine($@"Sent {count} messages in {sw.ElapsedMilliseconds} ms.");

        foreach (var participant in provider.GetServices<IParticipant<MessageWritten>>())
        {
            Assert.That(((AbstractParticipant)participant).CallCount, Is.EqualTo(count));
        }
    }

    [Test]
    public async Task Should_be_able_to_add_participant_with_multiple_implementations_async()
    {
        var messageTracker = new MessageTracker();

        var provider = new ServiceCollection()
            .AddMediator()
            .AddParticipantsFrom(GetType().Assembly)
            .Services
            .AddSingleton<IMessageTracker>(messageTracker)
            .AddSingleton<MultipleParticipants, MultipleParticipants>()
            .BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new MultipleParticipantMessageA());
        await mediator.SendAsync(new MultipleParticipantMessageB());
        await mediator.SendAsync(new MultipleParticipantMessageA());

        _ = provider.GetRequiredService<MultipleParticipants>();

        Assert.That(messageTracker.MessageTypeCount<MultipleParticipantMessageA>(), Is.EqualTo(2));
        Assert.That(messageTracker.MessageTypeCount<MultipleParticipantMessageB>(), Is.EqualTo(1));
    }
}