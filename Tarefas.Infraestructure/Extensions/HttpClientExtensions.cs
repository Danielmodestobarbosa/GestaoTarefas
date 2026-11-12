
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Tarefas.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpResilience(
            this IServiceCollection services,
            Action<HttpStandardResilienceOptions>? configure = null)
        {
            services
                .AddHttpClient("ServicoExterno")
                .AddStandardResilienceHandler(options =>
                {
                    options.TotalRequestTimeout.Timeout =
                        TimeSpan.FromSeconds(30);

                    options.AttemptTimeout.Timeout =
                        TimeSpan.FromSeconds(10);

                    options.Retry.MaxRetryAttempts = 3;

                    options.Retry.Delay =
                        TimeSpan.FromSeconds(2);

                    options.Retry.DisableForUnsafeHttpMethods();

                    configure?.Invoke(options);
                });

            return services;
        }
    }
}


