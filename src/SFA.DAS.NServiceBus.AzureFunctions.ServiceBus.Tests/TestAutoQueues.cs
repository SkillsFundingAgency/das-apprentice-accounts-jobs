using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunctions.ServiceBus.Tests;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[assembly: NServiceBusTriggerFunction(TestAutoQueues.EndpointName)]

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus.Tests
{
    public class TestAutoQueues
    {
        public const string EndpointName = "TheQueues";
        [Test]
        public async Task Create_queue_when_it_does_not_exist()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.QueueExistsAsync(EndpointName, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateQueueAsync(EndpointName, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Ignores_queue_when_it_does_already_exists()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.QueueExistsAsync(EndpointName, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateQueueAsync(EndpointName, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Create_error_queue_when_it_does_not_exist()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.QueueExistsAsync($"{EndpointName}-error", It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateQueueAsync($"{EndpointName}-error", It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Ignores_error_queue_when_it_does_already_exists()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.QueueExistsAsync($"{EndpointName}-error", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateQueueAsync($"{EndpointName}-error", It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Creates_subscription_when_it_does_not_exist()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.SubscriptionExistsAsync("bundle-1", EndpointName, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateSubscriptionAsync(
                It.Is<SubscriptionDescription>(d =>
                    d.TopicPath == "bundle-1" &&
                    d.SubscriptionName == EndpointName),
                It.Is<RuleDescription>(r => r.Filter is FalseFilter),
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Ignores_subscription_when_it_already_exists()
        {
            var m = new Mock<ManagementClient>("Endpoint=sb://bob.windows.net/;Authentication=Managed Identity;");
            m.Setup(x => x.SubscriptionExistsAsync("bundle-1", EndpointName, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await AutoSubscribeToQueues.CreateQueuesWithReflection(Assembly.GetExecutingAssembly(), m.Object);

            m.Verify(x => x.CreateSubscriptionAsync(
                It.Is<SubscriptionDescription>(d =>
                    d.TopicPath == "bundle-1" &&
                    d.SubscriptionName == EndpointName),
                It.Is<RuleDescription>(r => r.Filter is FalseFilter),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
