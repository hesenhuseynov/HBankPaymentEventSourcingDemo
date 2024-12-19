using EnigmaPaymentEventSourcing.Application.Interfacess;
using System.Collections.Concurrent;

namespace EnigmaPaymentEventSourcing.Infrastructure.ReadModel
{
    public class AccountReadModel
    {
        public Guid AccountId { get; set; }
        public string OwnerName { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public bool IsClosed { get; set; }
    }

    public class InMemoryReadModelStore : IAccountReadModel
    {
        private readonly ConcurrentDictionary<Guid, AccountReadModel> _accounts = new();

        public void CreateAccount(Guid accountId, string ownerName, string currency, decimal balance)
        {
            _accounts[accountId] = new AccountReadModel
            {
                AccountId = accountId,
                OwnerName = ownerName,
                Currency = currency,
                Balance = balance,
                IsClosed = false
            };
        }

        public void UpdateBalance(Guid accountId, decimal newBalance)
        {
            if (_accounts.TryGetValue(accountId, out var model))
            {
                model.Balance = newBalance;
            }
        }

        public void CloseAccount(Guid accountId)
        {
            if (_accounts.TryGetValue(accountId, out var model))
            {
                model.IsClosed = true;
            }
        }

        public AccountReadModel GetAccount(Guid accountId)
        {
            _accounts.TryGetValue(accountId, out var model);
            return model;
        }

        public IEnumerable<AccountReadModel> GetAllAccounts() => _accounts.Values.ToList();

        public Task<decimal?> GetBalanceAsync(Guid accountId)
        {
            _accounts.TryGetValue(accountId, out var model);
            decimal? balance = model?.Balance;
            return Task.FromResult(balance);
        }
    }

}
