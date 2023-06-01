using SFA.DAS.ApprenticeAccounts.Jobs.MockServer;

    OuterApiBuilder.Create(5123)
        .WithApprenticePatch()
        .WithApprenticeshipConfirmation()
        .Build();

    Console.WriteLine("Press any key to stop the servers");
    Console.ReadKey();