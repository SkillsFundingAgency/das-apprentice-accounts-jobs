using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.ObjectBuilder;
using SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure;
using System;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
[assembly: NServiceBusTriggerFunction(Startup.EndpointName)]

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    internal class Startup : FunctionsStartup
    {
        public const string EndpointName = "Bob";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //AutoSubscribeToQueues.CreateQueuesWithReflection(
            //    builder.GetContext().Configuration)
            //    .GetAwaiter().GetResult();

            //builder.Services.AddSingleton<InProcessFunctionEndpoint>

            builder.UseNServiceBus(() =>
            {
                var configuration = new ServiceBusTriggeredEndpointConfiguration(
                                endpointName: EndpointName,
                                connectionStringName: "NServiceBusConnectionString");
                configuration.AdvancedConfiguration.SendFailedMessagesTo($"{EndpointName}-error");
                configuration.LogDiagnostics();
                return configuration;
            });

            builder.Services.Decorate<IFunctionEndpoint>((ep, sp) =>
            {
                return ep;
            });
            //builder.Services.

            //builder.Services.AddSingleton<AutoStartableEndpointWithExternallyManagedContainer>();
            //builder.Services.Decorate<IStartableEndpointWithExternallyManagedContainer, AutoStartableEndpointWithExternallyManagedContainer>();
        }
    }

    internal class AutoStartableEndpointWithExternallyManagedContainer : IStartableEndpointWithExternallyManagedContainer
    {
        private readonly IStartableEndpointWithExternallyManagedContainer decorated;

        public AutoStartableEndpointWithExternallyManagedContainer(
            IStartableEndpointWithExternallyManagedContainer decorated)
            => this.decorated = decorated;

        public Lazy<NServiceBus.IMessageSession> MessageSession => decorated.MessageSession;

        public Task<IEndpointInstance> Start(IBuilder builder)
        {
            return decorated.Start(builder);
        }
    }

    //internal class AutoCreateFunctionEndpoint : FunctionEndpoint
    //{
    //    private readonly IFunctionEndpoint decorated;

    //    public AutoCreateFunctionEndpoint(IStartableEndpointWithExternallyManagedContainer externallyManagedContainerEndpoint, ServiceBusTriggeredEndpointConfiguration configuration, IServiceProvider serviceProvider)
    //        : base(externallyManagedContainerEndpoint, configuration, serviceProvider)
    //    {
    //    }

    //    //public AutoCreateFunctionEndpoint(IFunctionEndpoint decorated) => this.decorated = decorated;

    //    public Task Process(Message message, ExecutionContext executionContext, IMessageReceiver messageReceiver, ILogger functionsLogger)
    //        => decorated.Process(message, executionContext, messageReceiver, functionsLogger);

    //    public Task Process(Message message, ExecutionContext executionContext, ILogger functionsLogger)
    //        => throw new NotImplementedException();

    //    public Task Publish(object message, PublishOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Publish(message, options, executionContext, functionsLogger);

    //    public Task Publish<T>(Action<T> messageConstructor, PublishOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Publish(messageConstructor, options, executionContext, functionsLogger);

    //    public Task Publish(object message, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Publish(message, executionContext, functionsLogger);

    //    public Task Publish<T>(Action<T> messageConstructor, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Publish(messageConstructor, executionContext, functionsLogger);

    //    public Task Send(object message, SendOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Send(message, options, executionContext, functionsLogger);

    //    public Task Send(object message, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Send(message, executionContext, functionsLogger);

    //    public Task Send<T>(Action<T> messageConstructor, SendOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Send(messageConstructor, options, executionContext, functionsLogger);

    //    public Task Send<T>(Action<T> messageConstructor, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Send(messageConstructor, executionContext, functionsLogger);

    //    public Task Subscribe(Type eventType, SubscribeOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Subscribe(eventType, options, executionContext, functionsLogger);

    //    public Task Subscribe(Type eventType, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Subscribe(eventType, executionContext, functionsLogger);

    //    public Task Unsubscribe(Type eventType, UnsubscribeOptions options, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Unsubscribe(eventType, options, executionContext, functionsLogger);

    //    public Task Unsubscribe(Type eventType, ExecutionContext executionContext, ILogger functionsLogger)
    //        => decorated.Unsubscribe(eventType, executionContext, functionsLogger);
    //}

}
