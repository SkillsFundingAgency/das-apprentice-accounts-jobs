using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.ApprenticeAccounts.Jobs.EventHandlers.LoginServiceEventHandlers;
using SFA.DAS.ApprenticeAccounts.Jobs.InternalMessages.Commands;
using SFA.DAS.ApprenticeAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Extensions;

const string queueName = "SFA.DAS.ApprenticeAccounts";

IConfiguration config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.development.json", optional: true)
    .Build();

var connectionString = config["NServiceBusConnection"];
if (connectionString is null)
    throw new NotSupportedException("NServiceBusConnection should contain ServiceBus connection string");


var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeAccounts");
endpointConfiguration.EnableInstallers();
endpointConfiguration.UseMessageConventions();
endpointConfiguration.UseNewtonsoftJsonSerializer();

endpointConfiguration.SendOnly();

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.AddRouting(routeSettings =>
{
    routeSettings.RouteToEndpoint(typeof(LoginEmailAddressUpdated), queueName);
    routeSettings.RouteToEndpoint(typeof(ApprenticeshipConfirmationConfirmed), queueName);
});

transport.ConnectionString(connectionString);

var endpointInstance = await Endpoint.Start(endpointConfiguration)
    .ConfigureAwait(false);

while (true)
{
    Console.Clear();
    Console.WriteLine("To Publish an Event please select the option...");
    Console.WriteLine("1. Publish ApprenticeEmailAddressChanged");
    Console.WriteLine("2. Send RemindApprenticeCommand");
    Console.WriteLine("X. Exit");

    var choice = Console.ReadLine()?.ToLower();
    var apprenticeId = Guid.NewGuid();

    switch (choice)
    {
        case "1":
            await PublishMessage(endpointInstance, new ApprenticeEmailAddressChanged { ApprenticeId = apprenticeId, ChangedOn = new DateTime() });
            break;
        case "2":
            await SendMessage(endpointInstance, new RemindApprenticeCommand { RegistrationId = apprenticeId });
            break;
        case "x":
            await endpointInstance.Stop();
            return;
    }
}

async Task PublishMessage(IMessageSession messageSession, object message)
{
    await messageSession.Publish(message);

    Console.WriteLine("Message published.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}

async Task SendMessage(IMessageSession messageSession, object message)
{
    await messageSession.Send(message);

    Console.WriteLine("Message sent.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}