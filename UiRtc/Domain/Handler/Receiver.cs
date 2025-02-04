using Microsoft.Extensions.DependencyInjection;
using UiRtc.Domain.Handler.Interface;
using UiRtc.Domain.Repository.Interface;

namespace UiRtc.Domain.Handler
{
    internal class ReceiverService(IServiceProvider services, IHandlerRepository consumerRepository) : IReceiverService
    {
        public async Task Invoke(string hubName, string methodName, dynamic param)
        {
            var consumers = consumerRepository.Get(hubName, methodName);

            foreach (var consumer in consumers) {

                var serviceInstances = services.GetServices(consumer.ConsumerInterface);
                foreach (var serviceInstance in serviceInstances)
                {
                    if (serviceInstance != null)
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
    }
}
