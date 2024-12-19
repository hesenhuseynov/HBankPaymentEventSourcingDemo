using EnigmaPaymentEventSourcing.Core.Events;
using EnigmaPaymentEventSourcing.Core.Exceptions;

namespace EnigmaPaymentEventSourcing.Core.Entities
{

    public class BankAccount
    {
        public Guid Id { get; private set; }
        public decimal Balance { get; private set; }
        public string OwnerName { get; private set; }
        public string Currency { get; private set; }
        public DateTime CreationDate { get; private set; }
        public bool IsClosed { get; private set; }

        public decimal DailyWithdrawalLimit { get; private set; } = decimal.MaxValue;

        private decimal _dailyWithdrawnAmount = 0m;
        private DateTime _lastWithdrawDate = DateTime.MinValue;

        private readonly List<object> _uncommittedChanges = new();

        private BankAccount() { }
        public static BankAccount LoadFromHistory(IEnumerable<object> events)
        {
            var account = new BankAccount();
            foreach (var e in events)
            {
                account.Apply(e);
            }
            return account;
        }

        public static BankAccount CreateNew(string ownerName, string currency)
        {
            var accountId = Guid.NewGuid();
            var createdEvent = new AccountCreated(accountId, ownerName, currency, DateTime.UtcNow);
            var acc = new BankAccount();
            acc.ApplyChange(createdEvent);
            return acc;
        }

        public void Deposit(decimal amount)
        {
            if (IsClosed)
                throw new DomainException("Cannot deposit to a closed account.");
            if (amount <= 0)
                throw new DomainException("Deposit amount must be greater than zero.");

            var depositedEvent = new MoneyDeposited(Id, amount, DateTime.UtcNow);
            ApplyChange(depositedEvent);
        }

        public void Withdraw(decimal amount)
        {
            if (IsClosed)
                throw new DomainException("Cannot withdraw from a closed account.");
            if (amount <= 0)
                throw new DomainException("Withdraw amount must be greater than zero.");
            if (Balance < amount)
                throw new DomainException("Insufficient balance.");

            ResetDailyWithdrawIfNewDay();

            if (_dailyWithdrawnAmount + amount > DailyWithdrawalLimit)
                throw new DomainException("Withdrawal limit exceeded.");

            var withdrawnEvent = new MoneyWithdrawn(Id, amount, DateTime.UtcNow);
            ApplyChange(withdrawnEvent);
        }

        public void CloseAccount()
        {
            if (IsClosed)
                throw new DomainException("Account is already closed.");
            var closedEvent = new AccountClosed(Id, DateTime.UtcNow);
            ApplyChange(closedEvent);
        }

        public void SetDailyWithdrawalLimit(decimal limit)
        {
            if (limit < 0)
                throw new DomainException("Limit cannot be negative.");

            var limitEvent = new WithdrawalLimitSet(Id, limit, DateTime.UtcNow);
            ApplyChange(limitEvent);
        }

        public IEnumerable<object> GetUncommittedChanges() => _uncommittedChanges.AsReadOnly();

        private void ApplyChange(object @event)
        {
            Apply(@event);
            _uncommittedChanges.Add(@event);
        }

        private void Apply(object @event)
        {
            switch (@event)
            {
                case AccountCreated e:
                    Id = e.AccountId;
                    OwnerName = e.OwnerName;
                    Currency = e.Currency;
                    CreationDate = e.CreatedOn;
                    Balance = 0m;
                    IsClosed = false;
                    break;
                case MoneyDeposited e:
                    Balance += e.Amount;
                    break;
                case MoneyWithdrawn e:
                    Balance -= e.Amount;
                    _dailyWithdrawnAmount += e.Amount;
                    _lastWithdrawDate = e.WithdrawnOn.Date;
                    break;
                case AccountClosed _:
                    IsClosed = true;
                    break;
                case WithdrawalLimitSet e:
                    DailyWithdrawalLimit = e.Limit;
                    break;
            }
        }

        private void ResetDailyWithdrawIfNewDay()
        {
            var today = DateTime.UtcNow.Date;
            if (_lastWithdrawDate < today)
            {
                _dailyWithdrawnAmount = 0m;
                _lastWithdrawDate = today;
            }
        }
    }
}
