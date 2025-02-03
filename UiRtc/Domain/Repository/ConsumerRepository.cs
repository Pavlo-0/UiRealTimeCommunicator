using System.Collections.Concurrent;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;
namespace UiRtc.Domain.Repository
{
    internal class ConsumerRepository : IConsumerRepository
    {
        private static ConcurrentBag<ConsumerRecord> _consumers = new ConcurrentBag<ConsumerRecord>();

        public void Add(ConsumerRecord record)
        {
            //TODO: check duplicates
            _consumers.Add(record);
        }

        public IEnumerable<ConsumerRecord> GetList(string hubName)
        {
            return _consumers.Where(r => r.HubName == hubName);
        }

        public ConsumerRecord Get(string hubName, string methodName)
        {
            return _consumers.Where(r => r.HubName == hubName && r.MethodName == methodName).FirstOrDefault();
        }
    }
}
