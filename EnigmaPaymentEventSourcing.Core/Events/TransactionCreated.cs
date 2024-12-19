using EnigmaPaymentEventSourcing.Core.Entities;

namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class TransactionCreated : TransactionEvent
    {
        public Guid TransactionId { get; }
        public Guid AccountId { get; }
        public decimal Amount { get; }
        public string Type { get; }
        public DateTime OccurredOn { get; }
        public TransactionCreated(Guid transactionId, Guid accountId, decimal amount, string type, DateTime occurredOn)
            : base(transactionId, accountId, occurredOn)
        {
            Amount = amount;
            Type = type;
        }

    }
}
