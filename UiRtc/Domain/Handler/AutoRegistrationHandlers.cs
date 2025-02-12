using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Repository.Records;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.Handler
{

    internal class AutoRegistrationHandlers()
    {
        public void RegisterConnections(IServiceCollection services,
                                        IConnectionRepository connectionRepository)
        {
            var assembly = Assembly.GetEntryAssembly()!;
            var typeConnection = typeof(IUiRtcConnection<>);
            var allConnectionImplmentationTypes = GetClassesImplementing([typeConnection], assembly);

            foreach (var connectionImplmentationTypes in allConnectionImplmentationTypes)
            {
                var hubName = connectionImplmentationTypes.GetGenericArguments()[0].Name;
                var interfaceImplementation = connectionImplmentationTypes.GetInterfaces()
                    .First(i => i.ReflectedType == typeConnection);

                var record = new ConnectionRecord(hubName, interfaceImplementation, connectionImplmentationTypes);

                connectionRepository.Add(record);
                services.AddTransient(record.ConInterfaceImplementation, record.ConImplementation);
            }
        }

        public void RegisterHandlers(IServiceCollection services, IHandlerRepository consumerRepository)
        {
            //Registering consumers
            var assembly = Assembly.GetEntryAssembly()!;

            var handlersImplmentationTypes = GetClassesImplementing(
            [
                typeof(IUiRtcHandler<>),
                typeof(IUiRtcHandler<,>)
            ],
            assembly);

            var handlersContextImplmentationTypes = GetClassesImplementing(
            [
                typeof(IUiRtcContextHandler<>),
                typeof(IUiRtcContextHandler<,>)
            ],
            assembly);

            RegisterHandlersByType(services, consumerRepository, handlersImplmentationTypes, isContextHandlers: false);
            RegisterHandlersByType(services, consumerRepository, handlersContextImplmentationTypes, isContextHandlers: true);
        }

        private static void RegisterHandlersByType(
            IServiceCollection services,
            IHandlerRepository consumerRepository,
            IEnumerable<Type> allConsumerImplmentationTypes,
            bool isContextHandlers)
        {
            foreach (var consumerImplmentationType in allConsumerImplmentationTypes)
            {
                var hubName = NameHelper.GetHubNameByContract(consumerImplmentationType);
                var methodName = NameHelper.GetMethodName(consumerImplmentationType);
                var record = new HandlerRecord(
                    hubName,
                    methodName,
                    consumerImplmentationType.GetInterfaces().First().GetGenericTypeDefinition(),
                    consumerImplmentationType.GetInterfaces().First(),
                    consumerImplmentationType,
                    isContextHandlers,
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
