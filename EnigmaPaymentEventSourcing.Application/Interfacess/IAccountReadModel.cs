namespace EnigmaPaymentEventSourcing.Application.Interfacess
{
    public interface IAccountReadModel
    {
        Task<decimal?> GetBalanceAsync(Guid accountId);
    }
}
