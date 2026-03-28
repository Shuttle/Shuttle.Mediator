using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediator(Action<MediatorBuilder>? builder = null)
        {
            Guard.AgainstNull(services);

            var mediatorBuilder = new MediatorBuilder(services);

            builder?.Invoke(mediatorBuilder);

            services.AddOptions();
            services.TryAddScoped<IMediator, Mediator>();
            services.AddSingleton<IParticipantDelegateProvider>(_ => new ParticipantDelegateProvider(mediatorBuilder.GetDelegates()));

            return services;
        }
    }
}