using System.Collections.Concurrent;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.Repository
{
    internal class ConnectionRepository : IConnectionRepository
    {
        private static ConcurrentBag<ConnectionRecord> _connections = new ConcurrentBag<ConnectionRecord>();

        public void Add(ConnectionRecord record)
        {
            _connections.Add(record);
        }

        public IEnumerable<ConnectionRecord> Get(string hubName)
        {
            return _connections.Where(r => r.HubName == hubName);
        }
    }
}
