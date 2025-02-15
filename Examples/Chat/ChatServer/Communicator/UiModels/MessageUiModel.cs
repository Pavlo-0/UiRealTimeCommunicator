using Tapper;

namespace Chat.Communicator.UiModels
{
    [TranspilationSource]
    public class MessageUiModel
    {
        public string AuthorName { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsPrivate { get; set; }
        public string? RecipientName { get; set; }
    }
}
