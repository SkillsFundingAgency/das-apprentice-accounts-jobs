using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.ApprenticeAccounts.Jobs.MockServer;

public class OuterApiBuilder
{
    private readonly WireMockServer _server;

    public OuterApiBuilder(int port)
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = port,
            UseSSL = true,
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
        });
    }

    public static OuterApiBuilder Create(int port)
    {
        return new OuterApiBuilder(port);
    }

    public OuterApi Build()
    {
        return new OuterApi(_server);
    }

    public OuterApiBuilder WithApprenticePatch()
    {
        _server.Given(
                Request.Create()
                    .WithPath("/apprentices/*")
                    .UsingPatch())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.Accepted));

        return this;
    }

    public OuterApiBuilder WithApprenticeshipConfirmation()
    {
        _server.Given(
                Request.Create()
                    .WithPath("/apprentices/*/my-apprenticeship")
                    .UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));

        return this;
    }
}
