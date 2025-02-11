using UiRtc.Domain.Repository.Records;

namespace UiRtc.Domain.Repository.Interface
{
    internal interface IConnectionRepository
    {
        void Add(ConnectionRecord record);
        IEnumerable<ConnectionRecord> Get(string hubName);
    }
}
