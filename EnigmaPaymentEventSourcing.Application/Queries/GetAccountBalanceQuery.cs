using EnigmaPaymentEventSourcing.Application.Interfacess;

namespace EnigmaPaymentEventSourcing.Application.Queries
{
    public class GetAccountBalanceQuery
    {
        public Guid AccountId { get; }
        public GetAccountBalanceQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }


    public class GetAccountBalanceQueryHandler
    {
        private readonly IAccountReadModel _accountReadModel;

        public GetAccountBalanceQueryHandler(IAccountReadModel accountReadModel)
        {
            _accountReadModel = accountReadModel;
        }

        public Task<decimal?> Handle(GetAccountBalanceQuery query)
        {
            return _accountReadModel.GetBalanceAsync(query.AccountId);
        }
    }
}
