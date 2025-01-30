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
        public void RegisterHandlers(IServiceCollection services, IConsumerRepository consumerRepository, IHubNameGenerator hubNameGenerator)
        {
            //Registering consumers
            var assembly = Assembly.GetEntryAssembly()!;

            var typeModelConsumer = typeof(IUiRtcHandler<,>);
            var typeConsumer = typeof(IUiRtcHandler<>);

            var allConsumerImplmentationTypes = GetClassesImplementing([typeModelConsumer, typeConsumer], assembly);

            foreach (var allConsumerImplmentationType in allConsumerImplmentationTypes)
            {
                var hubName = hubNameGenerator.GetHubName(allConsumerImplmentationType.GetInterfaces().First().GenericTypeArguments[0]);
                var methodName = allConsumerImplmentationType.Name;
                var record = new ConsumerRecord(
                    hubName,
                    methodName,
                    allConsumerImplmentationType.GetInterfaces().First().GetGenericTypeDefinition(),
                    allConsumerImplmentationType.GetInterfaces().First(),
                    allConsumerImplmentationType,
                    allConsumerImplmentationType.GetInterfaces().First().GenericTypeArguments.Count() > 1 ?
                    allConsumerImplmentationType.GetInterfaces().First().GenericTypeArguments[1] : null
                    );

                consumerRepository.Add(record);

                services.AddTransient(record.ConsumerInterface, allConsumerImplmentationType);
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
