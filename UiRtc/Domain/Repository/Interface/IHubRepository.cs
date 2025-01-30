namespace UiRtc.Domain.Repository.Interface
{
    internal interface IHubRepository
    {
        void AddHub(string hubName, Type HubType);
        Type GetHubType(string hubName);
    }
}
