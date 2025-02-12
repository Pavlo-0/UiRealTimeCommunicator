namespace UiRtc.Domain.Repository.Records
{
    internal record HandlerRecord(string HubName,
                                   string MethodName,
                                   Type ConsumerInterfaceDefenition,
                                   Type ConsumerInterface,
                                   Type ConsumerImplementation,
                                   bool IsContextHandler,
                                   Type? GenericModel);
}
