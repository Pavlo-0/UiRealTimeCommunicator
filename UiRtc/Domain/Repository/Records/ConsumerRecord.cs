namespace UiRtc.Domain.Repository.Records
{
    internal record ConsumerRecord(string HubName, string MethodName, Type ConsumerInterfaceDefenition, Type ConsumerInterface, Type ConsumerImplementation, Type? GenericModel);
}
