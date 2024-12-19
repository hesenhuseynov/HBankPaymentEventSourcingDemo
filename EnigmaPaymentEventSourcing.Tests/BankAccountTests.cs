using EnigmaPaymentEventSourcing.Application.Commands;
using EnigmaPaymentEventSourcing.Application.Interfacess;
using EnigmaPaymentEventSourcing.Application.Queries;
using EnigmaPaymentEventSourcing.Core.Entities;
using EnigmaPaymentEventSourcing.Core.Exceptions;
using EnigmaPaymentEventSourcing.Core.Interfaces;
using EnigmaPaymentEventSourcing.Infrastructure.ReadModel;
using EnigmaPaymentEventSourcing.Infrastructure.Stores;
using static EnigmaPaymentEventSourcing.Application.Commands.DepositMoneyCommand;

namespace EnigmaPaymentEventSourcing.Tests
{
    public class BankAccountTests
    {
        private readonly IEventStore _eventStore;
        private readonly ITransactionStore _transactionStore;
        private readonly InMemoryReadModelStore _readModelStore;
        private readonly IProjectionUpdater _projectionUpdater;

        public BankAccountTests()
        {
            _eventStore = new EventStore();
            _transactionStore = new TransactionStore();
            _readModelStore = new InMemoryReadModelStore();
            _projectionUpdater = new ProjectionService(_readModelStore);
        }

        [Fact]
        public async Task DepositMoney_Should_Create_MoneyDeposited_Event_And_Update_Balance()
        {
            var createHandler = new CreateAccountCommandHandler(_eventStore, _projectionUpdater);
            var accountId = await createHandler.Handle(new CreateAccountCommand("Hasan", "manat"));

            var depositHandler = new DepositMoneyCommandHandler(_eventStore, _projectionUpdater);
            await depositHandler.Handle(new DepositMoneyCommand(accountId, 100m));

            var (events, version) = await _eventStore.GetEventsAsync(accountId);
            Assert.Contains(events, e => e.GetType().Name == "MoneyDeposited");

            var queryHandler = new GetAccountBalanceQueryHandler(_readModelStore);
            var balance = await queryHandler.Handle(new GetAccountBalanceQuery(accountId));
            Assert.Equal(100m, balance);
        }

        [Fact]
        public async Task Withdraw_More_Than_Balance_Should_Throw_DomainException()
        {
            var createHandler = new CreateAccountCommandHandler(_eventStore, _projectionUpdater);
            var accountId = await createHandler.Handle(new CreateAccountCommand("Hasan Huseynov", "manat"));

            var depositHandler = new DepositMoneyCommandHandler(_eventStore, _projectionUpdater);
            await depositHandler.Handle(new DepositMoneyCommand(accountId, 50m));

            var withdrawHandler = new WithdrawMoneyCommandHandler(_eventStore, _projectionUpdater);
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await withdrawHandler.Handle(new WithdrawMoneyCommand(accountId, 100m));
            });
        }

        [Fact]
        public async Task SetWithdrawalLimit_And_Exceed_It_Should_Throw_Exception()
        {
            var createHandler = new CreateAccountCommandHandler(_eventStore, _projectionUpdater);
            var accountId = await createHandler.Handle(new CreateAccountCommand("Hasan Huseynov", "manat"));

            var setLimitHandler = new SetWithdrawalLimitCommandHandler(_eventStore, _projectionUpdater);
            await setLimitHandler.Handle(new SetWithdrawalLimitCommand(accountId, 20m));

            var depositHandler = new DepositMoneyCommandHandler(_eventStore, _projectionUpdater);
            await depositHandler.Handle(new DepositMoneyCommand(accountId, 50m));

            var withdrawHandler = new WithdrawMoneyCommandHandler(_eventStore, _projectionUpdater);
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await withdrawHandler.Handle(new WithdrawMoneyCommand(accountId, 30m));
            });
        }
        [Fact]
        public async Task Concurrency_Should_Throw_ConcurrencyException_If_Versions_Do_Not_Match()
        {
            var createHandler = new CreateAccountCommandHandler(_eventStore, _projectionUpdater);
            var accountId = await createHandler.Handle(new CreateAccountCommand("Hasan Huseynov", "manat"));

            var depositHandler = new DepositMoneyCommandHandler(_eventStore, _projectionUpdater);
            await depositHandler.Handle(new DepositMoneyCommand(accountId, 100m));

            var (events, version) = await _eventStore.GetEventsAsync(accountId);
            var account = BankAccount.LoadFromHistory(events);

            await depositHandler.Handle(new DepositMoneyCommand(accountId, 50m));

            account.Deposit(25m);
            var changes = account.GetUncommittedChanges();

            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
            {
                await _eventStore.SaveEventsAsync(accountId, changes, version);
            });
        }
        [Fact]
        public async Task After_Account_Is_Closed_Deposit_Should_Throw_DomainException()
        {
            var createHandler = new CreateAccountCommandHandler(_eventStore, _projectionUpdater);
            var accountId = await createHandler.Handle(new CreateAccountCommand("Hasan", "manat"));

            var closeHandler = new CloseAccountCommandHandler(_eventStore, _projectionUpdater);
            await closeHandler.Handle(new CloseAccountCommand(accountId));

            var depositHandler = new DepositMoneyCommandHandler(_eventStore, _projectionUpdater);
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await depositHandler.Handle(new DepositMoneyCommand(accountId, 100m));
            });
        }

    }


}
