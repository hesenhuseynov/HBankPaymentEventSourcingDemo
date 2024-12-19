using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{
    public class SetWithdrawalLimitCommand
    {

        public Guid AccountId { get; }
        public decimal Limit { get; }

        public SetWithdrawalLimitCommand(Guid accountId, decimal limit)
        {
            AccountId = accountId;
            Limit = limit;
        }
    }



    public class SetWithdrawalLimitCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionUpdater _projectionUpdater;

        public SetWithdrawalLimitCommandHandler(IEventStore eventStore, IProjectionUpdater projectionUpdater)
        {
            _eventStore = eventStore;
            _projectionUpdater = projectionUpdater;
        }

        public async Task Handle(SetWithdrawalLimitCommand command)
        {
            var (events, version) = await _eventStore.GetEventsAsync(command.AccountId);
            var account = BankAccount.LoadFromHistory(events);

            account.SetDailyWithdrawalLimit(command.Limit);
            var changes = account.GetUncommittedChanges();

            await _eventStore.SaveEventsAsync(command.AccountId, changes, version);
            _projectionUpdater.UpdateProjection(changes);
        }
    }
}
