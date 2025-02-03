using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.Handler
{

    internal class AutoRegistrationHandlers()
    {
        public void RegisterHandlers(IServiceCollection services, IConsumerRepository consumerRepository)
        {
            //Registering consumers
            var assembly = Assembly.GetEntryAssembly()!;

            var typeModelConsumer = typeof(IUiRtcHandler<,>);
            var typeConsumer = typeof(IUiRtcHandler<>);

            var allConsumerImplmentationTypes = GetClassesImplementing([typeModelConsumer, typeConsumer], assembly);

            foreach (var consumerImplmentationType in allConsumerImplmentationTypes)
            {
                var hubName = NameHelper.GetHubNameByContract(consumerImplmentationType);
                var methodName = NameHelper.GetMethodName(consumerImplmentationType);
                var record = new ConsumerRecord(
                    hubName,
                    methodName,
                    consumerImplmentationType.GetInterfaces().First().GetGenericTypeDefinition(),
                    consumerImplmentationType.GetInterfaces().First(),
                    consumerImplmentationType,
                    consumerImplmentationType.GetInterfaces().First().GenericTypeArguments.Count() > 1 ?
                    consumerImplmentationType.GetInterfaces().First().GenericTypeArguments[1] : null
                    );

                consumerRepository.Add(record);

                services.AddTransient(record.ConsumerInterface, consumerImplmentationType);
            }
        }

        static IEnumerable<Type> GetClassesImplementing(Type[] targetInterfaceTypes, Assembly assembly)
        {
            // Get the target type (interface)
            // Return all interfaces in the assembly that implement the target type

            var matchingTypes = assembly.GetTypes()
                           .Where(type => type.IsClass && !type.IsAbstract) // Only non-abstract classes
                           .Where(type => type.GetInterfaces()
                               .Any(i =>
                                   i.IsGenericType &&
                                   targetInterfaceTypes.Contains(i.GetGenericTypeDefinition())
                                   ))
                           .ToList();
            return matchingTypes;
        }
    }
}
