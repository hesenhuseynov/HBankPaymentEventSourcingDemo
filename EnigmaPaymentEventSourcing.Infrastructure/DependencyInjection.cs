using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Interfaces;
using EnigmaPaymentEventSourcing.Infrastructure.ReadModel;
using EnigmaPaymentEventSourcing.Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace EnigmaPaymentEventSourcing.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IEventStore, EventStore>();
            services.AddSingleton<ITransactionStore, TransactionStore>();

            services.AddSingleton<IAccountReadModel, InMemoryReadModelStore>();

            services.AddSingleton<IProjectionUpdater, ProjectionService>();

            return services;
        }
    }
}
