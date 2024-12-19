using EnigmaPaymentEventSourcing.Core.Interfaces;
using System.Collections.Concurrent;

namespace EnigmaPaymentEventSourcing.Infrastructure.Stores
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }

    public class EventStore : IEventStore
    {
        private class AggregateData
        {
            public List<object> Events { get; set; } = new();
            public int Version { get; set; } = 0;
        }

        private readonly ConcurrentDictionary<Guid, AggregateData> _store = new();

        public Task<(IEnumerable<object> Events, int Version)> GetEventsAsync(Guid aggregateId)
        {
            if (!_store.TryGetValue(aggregateId, out var data))
                return Task.FromResult((Enumerable.Empty<object>(), 0));

            return Task.FromResult((data.Events.AsEnumerable(), data.Version));
        }

        public Task SaveEventsAsync(Guid aggregateId, IEnumerable<object> events, int expectedVersion)
        {
            var data = _store.GetOrAdd(aggregateId, new AggregateData());

            if (data.Version != expectedVersion)
                throw new ConcurrencyException($"Expected version {expectedVersion}, but found {data.Version} for aggregate {aggregateId}");

            var eventList = events.ToList();
            if (eventList.Any())
            {
                data.Events.AddRange(eventList);
                data.Version += eventList.Count;
            }

            return Task.CompletedTask;
        }
    }
}
