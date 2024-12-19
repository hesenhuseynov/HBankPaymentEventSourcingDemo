namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class AccountClosed
    {
        public Guid AccountId { get; }
        public DateTime ClosedOn { get; }
        public AccountClosed(Guid accountId, DateTime closedOn)
        {
            AccountId = accountId;
            ClosedOn = closedOn;
        }
    }
}
