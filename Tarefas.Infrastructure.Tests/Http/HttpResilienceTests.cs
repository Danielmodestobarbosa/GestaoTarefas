
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Polly.Timeout;
using Tarefas.Infrastructure.Extensions;

namespace Tarefas.Infrastructure.Tests.Extensions
{
    public class HttpClientExtensionsTests
    {
        [Fact(DisplayName = "Deve repetir requisição GET após falhas temporárias")]
        public async Task Deve_Repetir_Get_Apos_Falhas_Temporarias()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler();

            var services = new ServiceCollection();

            services.AddHttpResilience(options =>
            {
                options.Retry.MaxRetryAttempts = 2;
                options.Retry.Delay = TimeSpan.Zero;
                options.Retry.UseJitter = false;
            });

            services.Configure<HttpClientFactoryOptions>(
                "ServicoExterno",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(
                        builder => builder.PrimaryHandler = handler);
                });

            using var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ServicoExterno");

            // Act
            var response = await client.GetAsync("https://teste.local");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);

            Assert.Equal(3, handler.RequestCount);
        }

        [Fact(DisplayName = "Não deve repetir requisição POST após falha")]
        public async Task Nao_Deve_Repetir_Post_Apos_Falha()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler();
            var services = new ServiceCollection();

            services.AddHttpResilience(options =>
            {
                options.Retry.MaxRetryAttempts = 2;
                options.Retry.Delay = TimeSpan.Zero;
                options.Retry.UseJitter = false;
            });

            services.Configure<HttpClientFactoryOptions>(
                "ServicoExterno",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(
                        builder => builder.PrimaryHandler = handler);
                });

            using var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ServicoExterno");

            // Act
            var response = await client.PostAsync(
                "https://teste.local",
                new StringContent("{}"));

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.ServiceUnavailable,
                response.StatusCode);

            Assert.Equal(1, handler.RequestCount);
        }

        [Fact(DisplayName = "Deve interromper requisição que ultrapassar o timeout")]
        public async Task Deve_Interromper_Requisicao_Apos_Timeout()
        {
            // Arrange
            var handler = new SlowHttpMessageHandler();
            var services = new ServiceCollection();

            services.AddHttpResilience(options =>
            {
                options.TotalRequestTimeout.Timeout =
                    TimeSpan.FromSeconds(2);

                options.AttemptTimeout.Timeout =
                    TimeSpan.FromMilliseconds(100);

                options.Retry.DisableForUnsafeHttpMethods();
                options.Retry.MaxRetryAttempts = 1;
            });

            services.Configure<HttpClientFactoryOptions>(
                "ServicoExterno",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(
                        builder => builder.PrimaryHandler = handler);
                });

            using var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ServicoExterno");

            // Act
            var exception = await Assert.ThrowsAsync<TimeoutRejectedException>(
                () => client.GetAsync("https://teste.local"));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(1, handler.RequestCount);
        }

        [Fact(DisplayName = "Deve interromper a operação ao atingir o timeout total")]
        public async Task Deve_Interromper_Operacao_Apos_Timeout_Total()
        {
            // Arrange
            var handler = new SlowHttpMessageHandler();
            var services = new ServiceCollection();

            services.AddHttpResilience(options =>
            {
                options.TotalRequestTimeout.Timeout =
                    TimeSpan.FromMilliseconds(500);

                options.AttemptTimeout.Timeout =
                    TimeSpan.FromMilliseconds(200);

                options.Retry.MaxRetryAttempts = 1;

                options.Retry.ShouldHandle =
                    _ => ValueTask.FromResult(false);
            });

            services.Configure<HttpClientFactoryOptions>(
                "ServicoExterno",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(
                        builder => builder.PrimaryHandler = handler);
                });

            using var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ServicoExterno");

            // Act
            var exception = await Assert.ThrowsAsync<TimeoutRejectedException>(
                () => client.GetAsync("https://teste.local"));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(1, handler.RequestCount);
        }

        [Fact(DisplayName = "Deve respeitar timeout total durante novas tentativas")]
        public async Task Deve_Respeitar_Timeout_Total_Durante_Retry()
        {
            // Arrange
            var handler = new SlowHttpMessageHandler();
            var services = new ServiceCollection();

            services.AddHttpResilience(options =>
            {
                options.TotalRequestTimeout.Timeout =
                    TimeSpan.FromMilliseconds(600);

                options.AttemptTimeout.Timeout =
                    TimeSpan.FromMilliseconds(200);

                options.Retry.MaxRetryAttempts = 3;

                options.Retry.Delay =
                    TimeSpan.FromMilliseconds(100);

                options.Retry.UseJitter = false;
            });

            services.Configure<HttpClientFactoryOptions>(
                "ServicoExterno",
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(
                        builder => builder.PrimaryHandler = handler);
                });

            using var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient("ServicoExterno");

            // Act
            var exception = await Assert.ThrowsAsync<TimeoutRejectedException>(
                () => client.GetAsync("https://teste.local"));

            // Assert
            Assert.NotNull(exception);

            Assert.InRange(
                handler.RequestCount,
                1,
                3);
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            public int RequestCount { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                RequestCount++;

                var statusCode = RequestCount <= 2
                    ? System.Net.HttpStatusCode.ServiceUnavailable
                    : System.Net.HttpStatusCode.OK;

                return Task.FromResult(
                    new HttpResponseMessage(statusCode));
            }
        }

        private sealed class SlowHttpMessageHandler : HttpMessageHandler
        {
            public int RequestCount { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                RequestCount++;

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);

                return new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK);
            }
        }
    }
}
