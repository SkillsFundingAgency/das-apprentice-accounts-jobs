using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunctions.ServiceBus;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs
{
    public class InstallQueues
    {
        private readonly IConfiguration configuration;

        public InstallQueues(IConfiguration configuration)
            => this.configuration = configuration;

        [FunctionName("InstallQueues")]
        public async Task Run([TimerTrigger("0 0 0 31 Jan *", RunOnStartup = true)] TimerInfo t, ILogger log)
            => await AutoSubscribeToQueues.CreateQueues(Assembly.GetExecutingAssembly(), configuration, logger: log);
    }
}
