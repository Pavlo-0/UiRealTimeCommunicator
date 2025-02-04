using Microsoft.AspNetCore.Builder;
using System.Reflection;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.HubMap
{
    internal class AutoRegistrationHubs(IHubRepository hubRepository,
                                        IHandlerRepository consumerRepository) : IAutoRegistrationHubs
    {
        public void RegisterHub(Assembly assembly, WebApplication app)
        {
            var hubs = GetDeclarationsByInterface(typeof(IUiRtcHub), assembly);

            foreach (var hub in hubs)
            {
                var isInterface = hub.IsInterface;
                if (!isInterface && hub.IsAbstract)
                {
                    throw new Exception("Hub can not be abstract class");
                }

                var hubName = NameHelper.GetHubName(hub);

                var methods = consumerRepository.GetList(hubName);
                var signalRHubType = SignalRHubBuilder.GenerateNewSignalRHub(hubName, methods);

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

        private static IEnumerable<Type> GetDeclarationsByInterface(Type targetType, Assembly assembly)
        {
            // Get the target type (interface)
            // Return all interfaces in the assembly that implement the target type
            return assembly.GetTypes()
                           .Where(targetType.IsAssignableFrom);
        }

        private class DummyHub:IUiRtcHub { }
    }
}
