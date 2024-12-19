namespace EnigmaPaymentEventSourcing.Core.Interfaces
{
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<object> events, int expectedVersion);

        Task<(IEnumerable<object> Events, int Version)> GetEventsAsync(Guid aggregateId);

    }



}
