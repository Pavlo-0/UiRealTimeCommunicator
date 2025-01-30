using Microsoft.Extensions.DependencyInjection;
using UiRtc.Domain.Handler.Interface;
using UiRtc.Domain.Repository.Interface;

namespace UiRtc.Domain.Handler
{

    internal class ReceiverService(IServiceProvider services, IConsumerRepository consumerRepository) : IReceiverService
    {
        public async Task Invoke(string hubName, string methodName, dynamic param)
        {
            var consumers = consumerRepository.Get(hubName, methodName);

            var serviceInstances = services.GetServices(consumers.ConsumerInterface);

            foreach (var serviceInstance in serviceInstances)
            {
                dynamic dynamicConsumer = serviceInstance;

                if (param != null)
                {
                    await dynamicConsumer.ConsumeAsync(param);
                }
                else
                {
                    await dynamicConsumer.ConsumeAsync();
                }
            }
        }
    }
}
