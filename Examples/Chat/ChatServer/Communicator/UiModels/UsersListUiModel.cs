using Tapper;

namespace Chat.Communicator.UiModels
{
    [TranspilationSource]
    public class UsersListUiModel
    {
        public IEnumerable<UserUiModel> List { get; set; }
    }
}
