namespace EnigmaPaymentEventSourcing.Core.Entities
{
    public abstract class TransactionEvent
    {
        public Guid TransactionId { get; }
        public Guid AccountId { get; }
        public DateTime OccurredOn { get; }

        protected TransactionEvent(Guid transactionId, Guid accountId, DateTime occurredOn)
        {
            TransactionId = transactionId;
            AccountId = accountId;
            OccurredOn = occurredOn;
        }
    }
}
