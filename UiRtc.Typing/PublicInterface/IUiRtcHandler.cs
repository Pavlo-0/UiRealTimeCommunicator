namespace UiRtc.Typing.PublicInterface
{
    public interface IUiRtcHandler<THub> where THub : IUiRtcHub
    {
        Task ConsumeAsync();
    }

    public interface IUiRtcHandler<THub, TConsumerModel> where THub : IUiRtcHub where TConsumerModel : class
    {
        Task ConsumeAsync(TConsumerModel Model);
    }
}
