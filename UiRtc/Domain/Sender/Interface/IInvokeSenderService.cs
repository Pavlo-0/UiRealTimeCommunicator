namespace UiRtc.Domain.Sender.Interface
{
    internal interface IInvokeSenderService : IInvokeService
    {
        void ResolveHub(string hubName);
    }
}
