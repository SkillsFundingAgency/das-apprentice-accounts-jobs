using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Infrastructure
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddTransientFromRegistration<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : TInterface
            => services.AddTransient<TInterface>(s => s.GetRequiredService<TImplementation>());

        public static IServiceCollection AddSingletonFromRegistration<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : TInterface
            => services.AddSingleton<TInterface>(s => s.GetRequiredService<TImplementation>());
    }
}