namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class WithdrawalLimitSet
    {
        public Guid AccountId { get; }
        public decimal Limit { get; }
        public DateTime SetOn { get; }

        public WithdrawalLimitSet(Guid accountId, decimal limit, DateTime setOn)
        {
            AccountId = accountId;
            Limit = limit;
            SetOn = setOn;
        }
    }
}
