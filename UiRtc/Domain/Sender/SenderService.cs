using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.Sender
{
    internal class SenderService : ISenderService
    {
        private readonly IInvokeSenderService invokeSenderService;
        private readonly IServiceProvider service;
        private readonly IHubNameGenerator hubNameGenerator;

        public SenderService(IInvokeSenderService invokeSenderService, IServiceProvider service, IHubNameGenerator hubNameGenerator)
        {
            this.invokeSenderService = invokeSenderService;
            this.service = service;
            this.hubNameGenerator = hubNameGenerator;
        }

        public TContract Send<TContract>() where TContract : IUiRtcHub
        {
            var serviceInstance = service.GetService(typeof(IInvokeSenderService));
            invokeSenderService.ResolveHub(hubNameGenerator.GetHubName(typeof(TContract)));
            return SendMethodBuilder<TContract>.Build(invokeSenderService);
        }
    }
}
