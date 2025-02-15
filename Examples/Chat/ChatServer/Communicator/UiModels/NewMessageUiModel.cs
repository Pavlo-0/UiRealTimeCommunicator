using Tapper;

namespace Chat.Communicator.UiModels
{
    [TranspilationSource]
    public class NewMessageUiModel
    {
        public string Message { get; set; }
        public Guid? RecipientId { get; set; }
    }
}
