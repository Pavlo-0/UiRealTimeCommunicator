namespace Chat.DataAccess.Models
{
    public record UserRecord(Guid Id, string ConnectionId, string Name, DateTime JoinDate);
}
