using System.Security.Claims;
using AuthenticatedAccessTokenFactoryExample.Backend.Communication.Models;
using Microsoft.AspNetCore.SignalR;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace AuthenticatedAccessTokenFactoryExample.Backend.Communication.Handlers;

[UiRtcMethod("SendAuthenticatedMessage")]
public class SendAuthenticatedMessageHandler(
    ILogger<SendAuthenticatedMessageHandler> logger,
    IUiRtcSenderService senderService) : IUiRtcContextHandler<ChatHub, ChatMessageRequest>
{
    public async Task ConsumeAsync(ChatMessageRequest model, IUiRtcProxyContext context)
    {
        var user = context.User;
        var userId = context.UserIdentifier;
        var connectionId = context.ConnectionId;

        if (user?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning("Rejected unauthenticated Chat message from connection {ConnectionId}", connectionId);
            throw new HubException("Authenticated users only.");
        }

        var userName = user.FindFirstValue(ClaimTypes.Name) ?? user.Identity.Name ?? "Authenticated user";

        logger.LogInformation(
            "Accepted Chat message from {UserName} ({UserId}) on connection {ConnectionId}",
            userName,
            userId,
            connectionId);

        await senderService.Send<ChatSenderContract>(connectionId).SendAuthenticatedMessage(new ChatMessageResponse
        {
            Message = $"Server received: {model.Message}",
            UserId = userId,
            UserName = userName,
            ConnectionId = connectionId,
            SentAtUtc = DateTime.UtcNow
        });
    }
}
