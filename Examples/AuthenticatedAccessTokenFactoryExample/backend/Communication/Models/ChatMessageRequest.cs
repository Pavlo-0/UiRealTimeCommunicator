using Tapper;

namespace AuthenticatedAccessTokenFactoryExample.Backend.Communication.Models;

[TranspilationSource]
public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
}
