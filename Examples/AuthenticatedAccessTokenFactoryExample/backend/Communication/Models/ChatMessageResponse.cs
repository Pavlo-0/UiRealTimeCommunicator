using Tapper;

namespace AuthenticatedAccessTokenFactoryExample.Backend.Communication.Models;

[TranspilationSource]
public class ChatMessageResponse
{
    public string Message { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string ConnectionId { get; set; } = string.Empty;

    public DateTime SentAtUtc { get; set; }
}
