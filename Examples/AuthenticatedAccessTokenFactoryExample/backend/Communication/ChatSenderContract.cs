using AuthenticatedAccessTokenFactoryExample.Backend.Communication.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace AuthenticatedAccessTokenFactoryExample.Backend.Communication;

public interface ChatSenderContract : IUiRtcSenderContract<ChatHub>
{
    [UiRtcMethod("AuthenticatedMessage")]
    Task SendAuthenticatedMessage(ChatMessageResponse response);
}
