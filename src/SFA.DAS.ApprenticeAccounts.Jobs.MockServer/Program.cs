using SFA.DAS.ApprenticeAccounts.Jobs.MockServer;

    OuterApiBuilder.Create(5801)
        .Build();

    Console.WriteLine("Press any key to stop the servers");
    Console.ReadKey();