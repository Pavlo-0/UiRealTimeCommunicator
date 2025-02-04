using System.Collections.Concurrent;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;
namespace UiRtc.Domain.Repository
{
    internal class HandlerRepository : IHandlerRepository
    {
        private static ConcurrentBag<HandlerRecord> _consumers = new ConcurrentBag<HandlerRecord>();

        public void Add(HandlerRecord record)
        {
            _consumers.Add(record);
        }

        public IEnumerable<HandlerRecord> GetList(string hubName)
        {
            return _consumers.Where(r => r.HubName == hubName);
        }

        public IEnumerable<HandlerRecord> Get(string hubName, string methodName)
        {
            return _consumers.Where(r => r.HubName == hubName && r.MethodName == methodName);
        }
    }
}
