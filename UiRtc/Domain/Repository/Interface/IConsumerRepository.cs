using UiRtc.Domain.Repository.Records;
namespace UiRtc.Domain.Repository.Interface
{
    internal interface IConsumerRepository
    {
        void Add(ConsumerRecord record);
        IEnumerable<ConsumerRecord> Get(string hubName);
        ConsumerRecord Get(string hubName, string methodName);
    }
}
