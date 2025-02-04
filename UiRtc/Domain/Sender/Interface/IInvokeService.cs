namespace UiRtc.Domain.Sender.Interface
{
    internal interface IInvokeService
    {
        Task Invoke(string method, object model);
    }
}
