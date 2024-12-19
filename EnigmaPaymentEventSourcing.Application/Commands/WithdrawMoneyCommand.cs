using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{
    public class WithdrawMoneyCommand
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public WithdrawMoneyCommand(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
    public class WithdrawMoneyCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionUpdater _projectionUpdater;

        public WithdrawMoneyCommandHandler(IEventStore eventStore, IProjectionUpdater projectionUpdater)
        {
            _eventStore = eventStore;
            _projectionUpdater = projectionUpdater;
        }

        public async Task Handle(WithdrawMoneyCommand command)
        {
            var (events, version) = await _eventStore.GetEventsAsync(command.AccountId);
            var account = BankAccount.LoadFromHistory(events);

            account.Withdraw(command.Amount);
            var changes = account.GetUncommittedChanges();

            await _eventStore.SaveEventsAsync(command.AccountId, changes, version);
            _projectionUpdater.UpdateProjection(changes);
        }
    }
}
