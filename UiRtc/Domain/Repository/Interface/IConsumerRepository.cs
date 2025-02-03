using UiRtc.Domain.Repository.Records;
namespace UiRtc.Domain.Repository.Interface
{
    internal interface IConsumerRepository
    {
        void Add(ConsumerRecord record);
        IEnumerable<ConsumerRecord> GetList(string hubName);
        ConsumerRecord Get(string hubName, string methodName);
    }
}
