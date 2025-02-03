namespace UiRtc.Domain.Repository.Records
{
    internal record HubRecord(string HubName, Type HubType, Type SignalRHubType, bool IsInterface);
}
