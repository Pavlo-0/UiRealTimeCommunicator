namespace UiRtc.Domain.Handler.Interface
{
    internal interface IReceiverService
    {
        Task Invoke(string hubName, string methodName, dynamic param);
    }
}
