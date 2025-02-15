using Chat.Communicator.UiModels;
using Chat.DataAccess.Models;
using Chat.DataAccess;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;
using UiRtc.Public;

namespace Chat.Communicator.Handlers
{
    [UiRtcMethod("Message")]
    public class NewMessageHandler(
        IRepository<MessageRecord> messageRepository,
        IRepository<UserRecord> userRepository,
        IUiRtcSenderService senderService) : IUiRtcContextHandler<ChatHub, NewMessageUiModel>
    {
        public Task ConsumeAsync(NewMessageUiModel Model, IUiRtcProxyContext context)
        {
            var currentUser = userRepository.GetAll().First(user => user.ConnectionId == context.ConnectionId);

            UserRecord? recipient = Model.RecipientId != null
                ? userRepository.GetAll().FirstOrDefault(user => user.Id == Model.RecipientId)
                : null;

            var recordMessage = new MessageRecord(
                Guid.NewGuid(),
                context.ConnectionId,
                Model.Message,
                currentUser.Name,
                currentUser.Id,
                recipient?.Id,
                DateTime.UtcNow
            );
            messageRepository.Add(recordMessage);


            if (recipient != null)
            {
                //Send private message
                senderService.Send<ChatContract>([recipient.ConnectionId, currentUser.ConnectionId]).SendMessage(new MessageUiModel
                {
                    AuthorName = currentUser.Name,
                    Message = recordMessage.Message,
                    CreateDate = recordMessage.CreateDate,
                    IsPrivate = true,
                    RecipientName = recipient.Name
                });
            }
            else
            {
                //Send public message 
                senderService.Send<ChatContract>().SendMessage(new MessageUiModel
                {
                    AuthorName = currentUser.Name,
                    Message = recordMessage.Message,
                    CreateDate = recordMessage.CreateDate,
                    IsPrivate = false,
                });
            }
            return Task.CompletedTask;
        }
    }
}
