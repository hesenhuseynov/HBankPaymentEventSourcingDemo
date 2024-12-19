namespace EnigmaPaymentEventSourcing.Core.Events
{
    public class AccountCreated
    {
        public Guid AccountId { get; }
        public string OwnerName { get; }
        public string Currency { get; }
        public DateTime CreatedOn { get; }

        public AccountCreated(Guid accountId, string ownerName, string currency, DateTime createdOn)
        {
            AccountId = accountId;
            OwnerName = ownerName;
            Currency = currency;
            CreatedOn = createdOn;
        }
    }
}
