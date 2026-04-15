using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Contract;

namespace Shuttle.Mediator;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public MediatorBuilder AddMediator(Action<MediatorOptions>? configureOptions = null)
        {
            var builder = new MediatorBuilder(Guard.AgainstNull(services));

            services.AddOptions();
            services.AddOptions<MediatorOptions>().Configure(options =>
            {
                configureOptions?.Invoke(options);
            });

            services.TryAddScoped<IMediator, Mediator>();
            services.AddSingleton<IParticipantDelegateProvider>(_ => new ParticipantDelegateProvider(builder.GetDelegates()));

            return builder;
        }
    }
}