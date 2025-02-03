using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.HubMap.Interface
{
    internal interface IHubService
    {
        Type GenerateNewSignalRHub(string hubName, IEnumerable<ConsumerRecord> methods);
    }
}
