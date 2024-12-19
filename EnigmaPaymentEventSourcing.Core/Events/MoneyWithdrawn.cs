namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class MoneyWithdrawn
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }
        public DateTime WithdrawnOn { get; }

        public MoneyWithdrawn(Guid accountId, decimal amount, DateTime withdrawnOn)
        {
            AccountId = accountId;
            Amount = amount;
            WithdrawnOn = withdrawnOn;
        }
    }
}
