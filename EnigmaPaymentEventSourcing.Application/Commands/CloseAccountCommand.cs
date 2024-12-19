using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{

    public class CloseAccountCommand
    {
        public Guid AccountId { get; }
        public CloseAccountCommand(Guid accountId)
        {
            AccountId = accountId;
        }
    }

    public class CloseAccountCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionUpdater _projectionUpdater;

        public CloseAccountCommandHandler(IEventStore eventStore, IProjectionUpdater projectionUpdater)
        {
            _eventStore = eventStore;
            _projectionUpdater = projectionUpdater;
        }

        public async Task Handle(CloseAccountCommand command)
        {
            var (events, version) = await _eventStore.GetEventsAsync(command.AccountId);
            var account = BankAccount.LoadFromHistory(events);

            account.CloseAccount();
            var changes = account.GetUncommittedChanges();

            await _eventStore.SaveEventsAsync(command.AccountId, changes, version);
            _projectionUpdater.UpdateProjection(changes);
        }
    }
}
