using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace BE01.IntegrationTest.Scenarios.ConnectionIdSender
{
    public class ConnectionIdSenderHub : IUiRtcHub
    {
    }

    public interface ConnectionIdSender: IUiRtcSenderContract<ConnectionIdSenderHub>
    {
        Task SendToSpecificUser(InfoModel model);
    }

    [UiRtcMethod("ConnectionIdRequest")]
    public class ConnectionIdRequestHandler(IUiRtcSenderService senderService) : IUiRtcContextHandler<ConnectionIdSenderHub>
    {
        public async Task ConsumeAsync(IUiRtcProxyContext context)
        {
            var connectionId = context.ConnectionId;
            await senderService.Send<ConnectionIdSender>(connectionId)
                         .SendToSpecificUser(new InfoModel() {  ConnectionId = connectionId});
        }
    }

    [TranspilationSource]
    public class InfoModel
    {
        public string? ConnectionId { get; set; }
    }
}
