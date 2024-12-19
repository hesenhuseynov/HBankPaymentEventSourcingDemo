using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Interfaces;
using System.Collections.Concurrent;

namespace EnigmaPaymentEventSourcing.Infrastructure.Stores
{
    public class TransactionStore : ITransactionStore
    {
        private readonly ConcurrentDictionary<Guid, List<TransactionEvent>> _store = new();

        public Task SaveTransactionAsync(TransactionEvent transactionEvent)
        {
            if (!_store.ContainsKey(transactionEvent.AccountId))
                _store[transactionEvent.AccountId] = new List<TransactionEvent>();

            _store[transactionEvent.AccountId].Add(transactionEvent);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TransactionEvent>> GetTransactionsAsync(Guid accountId)
        {
            if (!_store.TryGetValue(accountId, out var transactions))
                return Task.FromResult(Enumerable.Empty<TransactionEvent>());

            return Task.FromResult(transactions.AsEnumerable());
        }
    }
}
