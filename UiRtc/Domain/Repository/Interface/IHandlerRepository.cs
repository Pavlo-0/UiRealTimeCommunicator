using UiRtc.Domain.Repository.Records;
namespace UiRtc.Domain.Repository.Interface
{
    internal interface IHandlerRepository
    {
        void Add(HandlerRecord record);
        IEnumerable<HandlerRecord> GetList(string hubName);
        IEnumerable<HandlerRecord> Get(string hubName, string methodName);
    }
}
