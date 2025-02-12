using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using UiRtc.Domain.Handler.Interface;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Public;

namespace UiRtc.Domain.Handler
{
    internal class ReceiverService(IServiceProvider services, IHandlerRepository consumerRepository) : IReceiverService
    {
        public async Task Invoke(string hubName, string methodName, HubCallerContext context, dynamic param)
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
                            if (!consumer.IsContextHandler) {
                                await dynamicConsumer.ConsumeAsync(param);
                            }

                            if (consumer.IsContextHandler)
                            {
                                await dynamicConsumer.ConsumeAsync(param, new ProxyContext(context));
                            }
                        }
                        else
                        {
                            if (!consumer.IsContextHandler)
                            {
                                await dynamicConsumer.ConsumeAsync();
                            }

                            if (consumer.IsContextHandler)
                            {
                                await dynamicConsumer.ConsumeAsync(new ProxyContext(context));
                            }
                        }
                    }
                }
            }
        }
    }
}
