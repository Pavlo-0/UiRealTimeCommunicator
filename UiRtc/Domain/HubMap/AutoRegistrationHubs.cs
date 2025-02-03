using Microsoft.AspNetCore.Builder;
using System.Reflection;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.HubMap
{
    internal class AutoRegistrationHubs(IHubService hubService,
                                        IHubRepository hubRepository,
                                        IConsumerRepository consumerRepository) : IAutoRegistrationHubs
    {
        public void RegisterHub(Assembly assembly, WebApplication app)
        {
            var hubs = GetDeclarationsByInterface<IUiRtcHub>(assembly);

            foreach (var hub in hubs)
            {
                var isInterface = hub.IsInterface;
                if (!isInterface && hub.IsAbstract)
                {
                    throw new Exception("Hub can be abstract class");
                }

                var hubName = NameHelper.GetHubName(hub);

                var methods = consumerRepository.GetList(hubName);
                var signalRHubType = hubService.GenerateNewSignalRHub(hubName, methods);

                var method = typeof(HubEndpointRouteBuilderExtensions)
                    .GetMethods()
                    .Where(method => method.Name == nameof(HubEndpointRouteBuilderExtensions.MapHub) &&
                                     method.GetParameters().Count() == 2 &&
                                     method.GetParameters().Last().ParameterType == typeof(string))
                    .First()
                    .MakeGenericMethod(signalRHubType);

                method?.Invoke(null, new object[] { app, hubName });

                hubRepository.AddHub(new Repository.Records.HubRecord(hubName, hub, signalRHubType, isInterface));
            }
        }

        static IEnumerable<Type> GetDeclarationsByInterface<TContract>(Assembly assembly)
        {
            // Get the target type (interface)
            var targetType = typeof(TContract);

            // Return all interfaces in the assembly that implement the target type
            return GetDeclarationsByInterface(targetType, assembly);
        }

        static IEnumerable<Type> GetDeclarationsByInterface(Type targetType, Assembly assembly)
        {
            // Get the target type (interface)
            // Return all interfaces in the assembly that implement the target type
            return assembly.GetTypes()
                           .Where(targetType.IsAssignableFrom);
        }

        static IEnumerable<Type> GetDeclarationsByInterfaceForHandler(Type targetType, Assembly assembly)
        {
            // Get the target type (interface)
            // Return all interfaces in the assembly that implement the target type
            return assembly.GetTypes()
                           .Where(t=> t.GetInterfaces().Select(i=> i.Name).Contains(targetType.Name));
        }

        private class DummyHub:IUiRtcHub { }
    }
}
