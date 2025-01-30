namespace UiRtc.Domain.Sender.Interface
{
    public interface IInvokeService
    {
        Task Invoke(string method, object model);
    }
}
