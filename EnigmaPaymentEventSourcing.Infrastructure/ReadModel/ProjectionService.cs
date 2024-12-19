using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Core.Events;

namespace EnigmaPaymentEventSourcing.Infrastructure.ReadModel
{
    public class ProjectionService : IProjectionUpdater
    {
        private readonly InMemoryReadModelStore _readModelStore;

        public ProjectionService(InMemoryReadModelStore readModelStore)
        {
            _readModelStore = readModelStore;
        }


        public void UpdateProjection(IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                Apply(e);
            }
        }

        private void Apply(object @event)
        {
            switch (@event)
            {
                case AccountCreated ev:
                    _readModelStore.CreateAccount(ev.AccountId, ev.OwnerName, ev.Currency, 0m);
                    break;
                case MoneyDeposited ev:
                    var accDep = _readModelStore.GetAccount(ev.AccountId);
                    if (accDep != null)
                        _readModelStore.UpdateBalance(ev.AccountId, accDep.Balance + ev.Amount);
                    break;
                case MoneyWithdrawn ev:
                    var accWit = _readModelStore.GetAccount(ev.AccountId);
                    if (accWit != null)
                        _readModelStore.UpdateBalance(ev.AccountId, accWit.Balance - ev.Amount);
                    break;
                case AccountClosed ev:
                    _readModelStore.CloseAccount(ev.AccountId);
                    break;
            }
        }
    }
}
