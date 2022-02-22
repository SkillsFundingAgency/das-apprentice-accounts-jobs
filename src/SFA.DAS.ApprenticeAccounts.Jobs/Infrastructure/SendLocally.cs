using NServiceBus;

namespace SFA.DAS.ApprenticeAccounts.Jobs
{
    public static class SendLocally
    {
        public static SendOptions Options
        {
            get
            {
                var options = new SendOptions();
                options.RouteToThisEndpoint();
                return options;
            }
        }
    }
}