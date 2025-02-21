﻿using Microsoft.AspNetCore.Builder;
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
        public static IServiceCollection AddUiRealTimeCommunicator(this IServiceCollection services, Action<IUiRtcConfiguration>? options = null)
        {
            var configuration = new Configuration();
            options?.Invoke(configuration); // Apply options if provided

            services.AddSignalR();

            services.AddSingleton<IUiRtcConfiguration>(configuration);
            services.AddTransient<IInvokeSenderService, InvokeSenderService>(); //Should be Transient
            services.AddTransient<IConnectionInvokeService, ConnectionInvokeService>(); //Should be Transient
            services.AddTransient<IUiRtcSenderService, SenderService>();

            services.AddTransient<IHubRepository, HubRepository>();

            services.AddTransient<IReceiverService, ReceiverService>();
            services.AddTransient<IAutoRegistrationHubs, AutoRegistrationHubs>();

            //Registering Handlers
            var handlerRepository = new HandlerRepository();
            services.AddSingleton<IHandlerRepository>(handlerRepository);

            var connectionRepository = new ConnectionRepository();
            services.AddSingleton<IConnectionRepository>(connectionRepository);

            var autoRegisterHandlers = new AutoRegistrationHandlers();
            autoRegisterHandlers.RegisterHandlers(services, handlerRepository);
            autoRegisterHandlers.RegisterConnections(services, connectionRepository);

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
