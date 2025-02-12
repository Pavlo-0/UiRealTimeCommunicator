namespace UiRtc.Typing.PublicInterface
{
    public interface IUiRtcContextHandler<THub> where THub : IUiRtcHub
    {
        Task ConsumeAsync(IUiRtcProxyContext context);
    }

    public interface IUiRtcContextHandler<THub, TConsumerModel> where THub : IUiRtcHub where TConsumerModel : class
    {
        Task ConsumeAsync(TConsumerModel Model, IUiRtcProxyContext context);
    }
}
