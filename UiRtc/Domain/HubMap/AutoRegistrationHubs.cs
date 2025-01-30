using Microsoft.AspNetCore.Builder;
using System.Reflection;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.HubMap
{
    internal class AutoRegistrationHubs(IHubNameGenerator hubNameGenerator,
                                        IHubService hubService,
                                        IHubRepository hubRepository,
                                        IConsumerRepository consumerRepository) : IAutoRegistrationHubs
    {
        public void RegisterHub(Assembly assembly, WebApplication app)
        {
            var contracts = GetInterfacesImplementingContract<IUiRtcHub>(assembly);

            foreach (var contract in contracts)
            {
                var hubName = hubNameGenerator.GetHubName(contract);
                var methods = consumerRepository.Get(hubName);
                var hubType = hubService.GenerateNewHub(hubName, methods);

                var method = typeof(HubEndpointRouteBuilderExtensions)
                    .GetMethods()
                    .Where(method => method.Name == nameof(HubEndpointRouteBuilderExtensions.MapHub) &&
                                     method.GetParameters().Count() == 2 &&
                                     method.GetParameters().Last().ParameterType == typeof(string))
                    .First()
                    .MakeGenericMethod(hubType);

                method?.Invoke(null, new object[] { app, hubName });

                hubRepository.AddHub(hubName, hubType);
            }
        }

        static IEnumerable<Type> GetInterfacesImplementingContract<TContract>(Assembly assembly)
        {
            // Get the target type (interface)
            var targetType = typeof(TContract);

            // Return all interfaces in the assembly that implement the target type
            return GetInterfacesImplementingContract(targetType, assembly);
        }

        static IEnumerable<Type> GetInterfacesImplementingContract(Type targetType, Assembly assembly)
        {
            // Get the target type (interface)
            // Return all interfaces in the assembly that implement the target type
            return assembly.GetTypes()
                           .Where(t => t.IsInterface && targetType.IsAssignableFrom(t));
        }
    }
}
