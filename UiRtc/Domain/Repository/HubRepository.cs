using System.Collections.Concurrent;
using UiRtc.Domain.Repository.Interface;

namespace UiRtc.Domain.Repository
{
    internal class HubRepository : IHubRepository
    {
        private static ConcurrentDictionary<string, Type> _hubs = new ConcurrentDictionary<string, Type>();

        public void AddHub(string hubName, Type HubType)
        {
            _hubs.TryAdd(hubName, HubType);
        }

        public Type GetHubType(string hubName)
        {
            return _hubs[hubName];
        }
    }
}
