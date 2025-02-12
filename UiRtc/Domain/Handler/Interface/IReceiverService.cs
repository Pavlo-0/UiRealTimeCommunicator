using Microsoft.AspNetCore.SignalR;

namespace UiRtc.Domain.Handler.Interface
{
    internal interface IReceiverService
    {
        Task Invoke(string hubName, string methodName, HubCallerContext context, dynamic param);
    }
}
