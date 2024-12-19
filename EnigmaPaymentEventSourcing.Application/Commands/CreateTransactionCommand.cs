using EnigmaPaymentEventSourcing.Core.Events;
using EnigmaPaymentEventSourcing.Core.Interfaces;

namespace EnigmaPaymentEventSourcing.Application.Commands
{
    public class CreateTransactionCommand
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }
        public string Type { get; }

        public CreateTransactionCommand(Guid accountId, decimal amount, string type)
        {
            AccountId = accountId;
            Amount = amount;
            Type = type;
        }
    }

    public class CreateTransactionCommandHandler
    {
        private readonly ITransactionStore _transactionStore;

        public CreateTransactionCommandHandler(ITransactionStore transactionStore)
        {
            _transactionStore = transactionStore;
        }

        public async Task Handle(CreateTransactionCommand command)
        {
            var transactionEvent = new TransactionCreated(
                Guid.NewGuid(),
                command.AccountId,
                command.Amount,
                command.Type,
                DateTime.UtcNow
            );

            await _transactionStore.SaveTransactionAsync(transactionEvent);
        }
    }
}
