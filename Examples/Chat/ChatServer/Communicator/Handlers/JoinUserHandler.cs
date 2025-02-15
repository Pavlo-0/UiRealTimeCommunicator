using Chat.Communicator.UiModels;
using Chat.DataAccess;
using Chat.DataAccess.Models;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace Chat.Communicator.Handlers
{
    [UiRtcMethod("Join")]
    public class JoinUserHandler(
        IRepository<UserRecord> userRepository,
        IUiRtcSenderService senderService) : IUiRtcContextHandler<ChatHub, NewUserRequestUiModel>
    {
        public Task ConsumeAsync(NewUserRequestUiModel model, IUiRtcProxyContext context)
        {
            var connectionId = context.ConnectionId;

            var oldUserRecord = userRepository.GetAll().Where(user => user.ConnectionId == connectionId).FirstOrDefault();
            if (oldUserRecord is not null)
            {
                userRepository.Remove(oldUserRecord);
                userRepository.Add(oldUserRecord with { Name = model.Name });
            }
            else
            {
                userRepository.Add(new UserRecord(Guid.NewGuid(), context.ConnectionId, model.Name, DateTime.UtcNow));
            }

            senderService.Send<ChatContract>().SendUsers(new UsersListUiModel()
            {
                List = userRepository.GetAll().Select(user => new UserUiModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                })
            });

            return Task.CompletedTask;
        }
    }
}
