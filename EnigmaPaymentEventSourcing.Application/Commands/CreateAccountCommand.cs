using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{
    public class CreateAccountCommand
    {

        public string OwnerName { get; }
        public string Currency { get; set; }

        public CreateAccountCommand(string ownerName, string currency)
        {
            OwnerName = ownerName;
            Currency = currency;
        }

    }

    public class CreateAccountCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IProjectionUpdater _projectionUpdater;

        public CreateAccountCommandHandler(IEventStore eventStore, IProjectionUpdater projectionUpdater)
        {
            _eventStore = eventStore;
            _projectionUpdater = projectionUpdater;
        }

        public async Task<Guid> Handle(CreateAccountCommand command)
        {
            var account = BankAccount.CreateNew(command.OwnerName, command.Currency);
            var changes = account.GetUncommittedChanges();

            await _eventStore.SaveEventsAsync(account.Id, changes, 0);

            _projectionUpdater.UpdateProjection(changes);

            return account.Id;
        }
    }
}
