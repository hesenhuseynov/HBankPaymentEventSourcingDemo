using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{
    public class DepositMoneyCommand
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public DepositMoneyCommand(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;

        }
        public class DepositMoneyCommandHandler
        {
            private readonly IEventStore _eventStore;
            private readonly IProjectionUpdater _projectionUpdater;

            public DepositMoneyCommandHandler(IEventStore eventStore, IProjectionUpdater projectionUpdater)
            {
                _eventStore = eventStore;
                _projectionUpdater = projectionUpdater;
            }

            public async Task Handle(DepositMoneyCommand command)
            {
                var (events, version) = await _eventStore.GetEventsAsync(command.AccountId);
                var account = BankAccount.LoadFromHistory(events);

                account.Deposit(command.Amount);
                var changes = account.GetUncommittedChanges();

                await _eventStore.SaveEventsAsync(command.AccountId, changes, version);
                _projectionUpdater.UpdateProjection(changes);
            }
        }
    }

}

