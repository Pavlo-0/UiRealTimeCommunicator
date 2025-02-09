using UiRtc.Domain.Sender.Interface;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.Sender
{
    internal class SenderService : IUiRtcSenderService
    {
        private readonly IInvokeSenderService invokeSenderService;
        private readonly IServiceProvider service;

        public SenderService(IInvokeSenderService invokeSenderService, IServiceProvider service)
        {
            this.invokeSenderService = invokeSenderService;
            this.service = service;
        }

        public TContract Send<TContract>() where TContract : IUiRtcSenderContract<IUiRtcHub>
        {
            var serviceInstance = service.GetService(typeof(IInvokeSenderService));
            invokeSenderService.ResolveHub(NameHelper.GetHubNameByContract(typeof(TContract)));
            return SendMethodBuilder<TContract>.Build(invokeSenderService);
        }
    }
}
