using Chat.Communicator.UiModels;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace Chat.Communicator
{
    public interface ChatContract : IUiRtcSenderContract<ChatHub>
    {
        [UiRtcMethod("OnMessage")]
        Task SendMessage(MessageUiModel message);

        [UiRtcMethod("UserListUpdate")]
        Task SendUsers(UsersListUiModel list);

        [UiRtcMethod("OnUpdate")]
        Task SendFullUpdate(FullUpdateUiModel update);
    }
}
