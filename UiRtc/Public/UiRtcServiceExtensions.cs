using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UiRtc.Domain.Handler;
using UiRtc.Domain.Handler.Interface;
using UiRtc.Domain.HubMap;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Sender;
using UiRtc.Domain.Sender.Interface;


namespace UiRtc.Public
{
    public static class UiRtcServiceExtensions
    {
        public static IServiceCollection AddUiRealTimeCommunicator(this IServiceCollection services, Action<IUiRtcConfiguration> options)
        {
            var configuration = new Configuration();
            options(configuration);

            services.AddSignalR();

            services.AddSingleton<IUiRtcConfiguration>(configuration);
            services.AddTransient<IInvokeSenderService, InvokeSenderService>(); //Should be Transient
            services.AddTransient<ISenderService, SenderService>();

            services.AddTransient<IHubService, HubService>();
            services.AddTransient<IHubRepository, HubRepository>();

            services.AddTransient<IReceiverService, ReceiverService>();
            services.AddTransient<IAutoRegistrationHubs, AutoRegistrationHubs>();

            //Registering Handlers
            var repository = new ConsumerRepository();
            services.AddSingleton<IConsumerRepository>(repository);

            var hubNameGenerator = new HubNameGenerator();
            services.AddSingleton<IHubNameGenerator>(hubNameGenerator);


            var autoRegisterHandlers = new AutoRegistrationHandlers();
            autoRegisterHandlers.RegisterHandlers(services, repository, hubNameGenerator);

            return services;
        }

        public static void UseUiRealTimeCommunicator(this WebApplication app)
        {
            var assembly = Assembly.GetEntryAssembly()!;

            var autoRegistrationHubs = app.Services.GetRequiredService<IAutoRegistrationHubs>();
            autoRegistrationHubs.RegisterHub(assembly, app);
        }
    }
}
