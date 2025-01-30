namespace UiRtc.Typing.PublicInterface
{
    public interface IUiRtcHandler<TContract> where TContract : IUiRtcHub
    {
        Task ConsumeAsync();
    }

    public interface IUiRtcHandler<TContract, TConsumerModel> where TContract : IUiRtcHub where TConsumerModel : class
    {
        Task ConsumeAsync(TConsumerModel Model);
    }

}
