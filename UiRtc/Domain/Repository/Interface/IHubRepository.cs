using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.Repository.Interface
{
    internal interface IHubRepository
    {
        void AddHub(HubRecord record);
        Type GetSignalRHubType(string hubName);
        Type GetHubType(string hubName);
    }
}
