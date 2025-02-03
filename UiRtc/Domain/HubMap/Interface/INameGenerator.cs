namespace UiRtc.Domain.HubMap.Interface
{
    internal interface INameGenerator
    {
        string GetHubName(Type type);
        string GetHubNameByContract(Type type);
        string GetMethodName(Type type);
    }
}
