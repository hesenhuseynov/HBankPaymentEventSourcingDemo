using EnigmaPaymentEventSourcing.Core.Entities;

namespace EnigmaPaymentEventSourcing.Core.Interfaces
{
    public interface ITransactionStore
    {
        Task SaveTransactionAsync(TransactionEvent transactionEvent);
        Task<IEnumerable<TransactionEvent>> GetTransactionsAsync(Guid accountId);
    }
}
