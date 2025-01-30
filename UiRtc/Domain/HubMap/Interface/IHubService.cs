using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.HubMap.Interface
{
    internal interface IHubService
    {
        Type GenerateNewHub(string hubName, IEnumerable<ConsumerRecord> methods);
    }
}
