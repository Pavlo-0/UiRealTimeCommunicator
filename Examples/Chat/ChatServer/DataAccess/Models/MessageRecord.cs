namespace Chat.DataAccess.Models
{
    public record MessageRecord(
        Guid Id,
        string ConnectionId,
        string Message,
        string AuthorName,
        Guid AuthorId,
        Guid? RecipientId,
        DateTime CreateDate);
}
