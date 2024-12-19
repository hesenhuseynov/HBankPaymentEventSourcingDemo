namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class MoneyDeposited
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }
        public DateTime DepositedOn { get; }

        public MoneyDeposited(Guid accountId, decimal amount, DateTime depositedOn)
        {
            AccountId = accountId;
            Amount = amount;
            DepositedOn = depositedOn;
        }




    }
}
