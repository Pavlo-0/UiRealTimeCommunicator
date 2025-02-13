using Tapper;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.OnConnection
{
    public class OnConnectionManager : IUiRtcHub
    {
    }

    public interface OnConnectionManagerSender : IUiRtcSenderContract<OnConnectionManager>
    {
        Task UpdateStatus(OnConnectionStatusModel status);
    }

    [TranspilationSource]
    public class OnConnectionStatusModel
    {
        public bool IsConnected { get; set; }
    }
}
