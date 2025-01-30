namespace UiRtc.Domain.Sender.Interface
{
    public interface IInvokeSenderService : IInvokeService
    {
        void ResolveHub(string hubName);
        //public Task Invoke(string method, object model);
    }
}
