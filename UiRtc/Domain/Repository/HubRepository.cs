using System.Collections.Concurrent;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.Repository
{
    internal class HubRepository : IHubRepository
    {
        private static ConcurrentBag<HubRecord> _hubs = new ConcurrentBag<HubRecord> ();

        public void AddHub(HubRecord record)
        {
            if (_hubs.Any(hub => hub.HubName == record.HubName))
            {
                throw new Exception($"Hub with {record.HubName} name has been registered. Can't be duplicate hub");
            }

            if (_hubs.Any(hub => hub.HubType == record.HubType))
            {
                throw new Exception($"Hub {record.HubName} with that concrete class or interface has been registered. Can't be duplicate hub");
            }

            _hubs.Add(record);
        }

        public Type GetHubType(string hubName)
        {
            var hub = _hubs.FirstOrDefault(hub => hub.HubName == hubName);

            if (hub == null)
            {
                throw new Exception("Hub hasn't been registered. Check proper declaration");
            }
            return hub.HubType;
        }

        public Type GetSignalRHubType(string hubName)
        {
            var hub = _hubs.FirstOrDefault(hub => hub.HubName == hubName);

            if (hub == null)
            {
                throw new Exception("Hub hasn't been registered. Check proper declaration");
            }
            return hub.SignalRHubType;
        }
    }
}
