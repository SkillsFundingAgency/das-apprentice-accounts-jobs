using Microsoft.Extensions.Configuration;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.ApprenticeCommitments.Messages.Events;

const string queueName = "SFA.DAS.ApprenticeAccounts";

IConfiguration config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.development.json", optional: false)
    .Build();

var connectionString = config["Values:NServiceBusConnectionString"] ?? throw new NotSupportedException("NServiceBusConnection should contain ServiceBus connection string");

var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeAccounts");
endpointConfiguration.EnableInstallers();
endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
endpointConfiguration.Conventions()
    .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"))
    .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"))
    .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));

endpointConfiguration.SendOnly();

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.Routing().RouteToEndpoint(typeof(UpdateEmailAddressCommand), queueName);

transport.ConnectionString(connectionString);

var endpointInstance = await Endpoint.Start(endpointConfiguration)
    .ConfigureAwait(false);

while (true)
{
    Console.Clear();
    Console.WriteLine("To Publish an Event please select the option...");
    Console.WriteLine("1. Send UpdateEmailAddressCommand");
    Console.WriteLine("2. Publish ApprenticeshipConfirmationConfirmedEvent");
    Console.WriteLine("X. Exit");

    var choice = Console.ReadLine()?.ToLower();
    var apprenticeId = Guid.NewGuid();
    var commitmentsApprenticeshipId = 1;

    switch (choice)
    {
        case "1":
            await SendMessage(endpointInstance,
                new UpdateEmailAddressCommand
                {
                    ApprenticeId = apprenticeId, CurrentEmailAddress = "current@test.com",
                    NewEmailAddress = "new@test.com"
                });
            break;
        case "2":
            await PublishMessage(endpointInstance, new ApprenticeshipConfirmationConfirmedEvent
            {
                ApprenticeId = apprenticeId,
                CommitmentsApprenticeshipId = commitmentsApprenticeshipId, 
                ConfirmedOn = DateTime.Now,
                CommitmentsApprovedOn = DateTime.Today.AddDays(-2)
            });
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