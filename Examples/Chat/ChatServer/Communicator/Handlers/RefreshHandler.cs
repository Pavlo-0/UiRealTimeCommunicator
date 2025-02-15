using Chat.DataAccess;
using Chat.DataAccess.Models;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace Chat.Communicator.Handlers
{
    public class RefreshHandler(
        IRepository<UserRecord> userRepository,
        IRepository<MessageRecord> messageRepository,
        IUiRtcSenderService senderService) : IUiRtcContextHandler<ChatHub>
    {
        public async Task ConsumeAsync(IUiRtcProxyContext context)
        {
            var listUsers = userRepository.GetAll();
            var currentUser = listUsers.First(user => user.ConnectionId == context.ConnectionId);
            var listMessages = messageRepository.GetAll().Where(message => 
            message.RecipientId is null || message.RecipientId == currentUser.Id || message.AuthorId == currentUser.Id);

            await senderService.Send<ChatContract>(context.ConnectionId).SendFullUpdate(
                new UiModels.FullUpdateUiModel()
                {
                    CurrentUser = new UiModels.UserUiModel()
                    {
                        Id = currentUser.Id,
                        Name = currentUser.Name
                    },
                    UsersList = listUsers.Select(user => new UiModels.UserUiModel()
                    {
                        Id = user.Id,
                        Name = user.Name
                    }),
                    MessagesList = listMessages.Select(message => new UiModels.MessageUiModel()
                    {
                        AuthorName = message.AuthorName,
                        Message = message.Message,
                        CreateDate = message.CreateDate,
                        IsPrivate = message.RecipientId is not null,
                    })
                });
        }
    }
}
