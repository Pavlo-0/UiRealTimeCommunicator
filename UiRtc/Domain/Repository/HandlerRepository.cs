using System.Collections.Concurrent;
using System.Linq;
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

        public IEnumerable<HandlerBuilderModel> GetBuilderList(string hubName)
        {
            return _consumers.Where(r => r.HubName == hubName).Select(
                handler => new HandlerBuilderModel(handler.MethodName, handler.GenericModel))
                .Distinct();
        }

        public IEnumerable<HandlerRecord> Get(string hubName, string methodName)
        {
            return _consumers.Where(r => r.HubName == hubName && r.MethodName == methodName);
        }
    }
}
