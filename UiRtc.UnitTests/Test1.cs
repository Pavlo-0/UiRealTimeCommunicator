using UiRtc.Domain.Sender;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace UiRtc.UnitTests
{
    [TestClass]
    public sealed class SenderServiceTests
    {
        [TestMethod]
        public void SendWithoutConnectionIdsAfterTargetedSendClearsTargeting()
        {
            var invokeSenderService = new RecordingInvokeSenderService();
            IUiRtcSenderService senderService = new SenderService(invokeSenderService, new EmptyServiceProvider());

            senderService.Send<ISenderTargetingContract>("connection-1");
            senderService.Send<ISenderTargetingContract>();

            Assert.AreEqual(2, invokeSenderService.ConnectionIdResolutions.Count);
            CollectionAssert.AreEqual(
                new[] { "connection-1" },
                invokeSenderService.ConnectionIdResolutions[0]);
            Assert.AreEqual(0, invokeSenderService.ConnectionIdResolutions[1].Length);
        }

        [UiRtcHub("SenderTargetingHub")]
        public sealed class SenderTargetingHub : IUiRtcHub
        {
        }

        public interface ISenderTargetingContract : IUiRtcSenderContract<SenderTargetingHub>
        {
            Task Notify(string message);
        }

        private sealed class RecordingInvokeSenderService : IInvokeSenderService
        {
            public List<string[]> ConnectionIdResolutions { get; } = new();

            public Task Invoke(string method, object model)
            {
                return Task.CompletedTask;
            }

            public void ResolveHub(string hubName)
            {
            }

            public void ResolveConnectionId(string[] userId)
            {
                ConnectionIdResolutions.Add(userId);
            }
        }

        private sealed class EmptyServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}
