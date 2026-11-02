
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tarefas.Api.Requests.Tarefas;
using Tarefas.Api.Tests.Infrastructure;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Domain.Tarefas.Enums;
using Xunit;

namespace Tarefas.Api.Tests.Controllers;

public sealed class TarefasControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TarefasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }


    [Fact(DisplayName = "Deve criar tarefa pela API")]
    public async Task Deve_Criar_Tarefa_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

        var jsonOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        var resultado = await response.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().NotBeEmpty();
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Descricao.Should().Be("Estudar testes de integração");
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);
    }

}
