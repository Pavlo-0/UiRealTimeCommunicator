using Tapper;

namespace Chat.Communicator.UiModels
{
    [TranspilationSource]
    public class FullUpdateUiModel
    {
        public IEnumerable<UserUiModel> UsersList { get; set; }
        public IEnumerable<MessageUiModel> MessagesList { get; set; }
        public UserUiModel CurrentUser { get; set; }
    }
}
