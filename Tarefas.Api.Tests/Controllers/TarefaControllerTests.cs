
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tarefas.Api.Requests.Tarefas;
using Tarefas.Api.Requests.Usuarios;
using Tarefas.Api.Tests.Infrastructure;
using Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;
using Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;
using Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;
using Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Queries.Tarefas.DTOs;
using Tarefas.Domain.Tarefas.Enums;
using Xunit;

namespace Tarefas.Api.Tests.Controllers;

public sealed class TarefasControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TarefasControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private readonly JsonSerializerOptions _jsonOptions =
    new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };


    [Fact(DisplayName = "Deve criar tarefa pela API")]
    public async Task Deve_Criar_Tarefa_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var jsonOptions = new JsonSerializerOptions(
            JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

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


    [Fact(DisplayName = "Deve obter tarefa pelo ID pela API")]
    public async Task Deve_Obter_Tarefa_Pelo_ID_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var criacaoResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

        var criacaoResultado = await criacaoResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        var tarefaId = criacaoResultado!.TarefaId;

        // Act
        var response = await _client.GetAsync(
            $"/api/tarefas/{tarefaId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content
            .ReadFromJsonAsync<ObterTarefaPorIdResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(tarefaId);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Descricao.Should().Be("Estudar testes de integração");
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta.ToString());
    }

    [Fact(DisplayName = "Deve retornar NotFound quando tarefa não encontrada")]
    public async Task Deve_Retornar_NotFound_Quando_Tarefa_Nao_Encontrada()
    {
        // Arrange
        var tarefaId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/tarefas/{tarefaId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve listar todas as tarefas pela API")]
    public async Task Deve_Listar_Tarefas_Pela_API()
    {
        // Arrange
        var request1 = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var request2 = new CriarTarefaRequest(
            "Estudar DDD",
            "Estudar arquitetura de software",
            PrioridadeTarefa.Media);

        await _client.PostAsJsonAsync("/api/tarefas", request1);
        await _client.PostAsJsonAsync("/api/tarefas", request2);

        // Act
        var response = await _client.GetAsync("/api/tarefas");

        var resultado = await response.Content
            .ReadFromJsonAsync<List<TarefaResumoDto>>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);

        resultado.Should().Contain(t =>
            t.Titulo == "Estudar C#" &&
            t.Status == StatusTarefa.Pendente.ToString());

        resultado.Should().Contain(t =>
            t.Titulo == "Estudar DDD" &&
            t.Status == StatusTarefa.Pendente.ToString());
    }


    [Fact(DisplayName = "Deve atualizar tarefa pela API")]
    public async Task Deve_Atualizar_Tarefa_Pela_API()
    {
        // Arrange
        var criarRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var criacaoResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            criarRequest);

        var criacaoResultado = await criacaoResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        var tarefaId = criacaoResultado!.TarefaId;

        var atualizarRequest = new AtualizarTarefaRequest(
            "Estudar DDD",
            "Estudar arquitetura de software",
            PrioridadeTarefa.Media);

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/tarefas/{tarefaId}",
            atualizarRequest);

        var resultado = await response.Content
            .ReadFromJsonAsync<AtualizarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefaId);
        resultado.Titulo.Should().Be("Estudar DDD");
        resultado.Descricao.Should().Be("Estudar arquitetura de software");
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Media);
    }


    [Fact(DisplayName = "Deve retornar NotFound ao atualizar tarefa inexistente")]
    public async Task Deve_Retornar_NotFound_Ao_Atualizar_Tarefa_Inexistente()
    {
        // Arrange
        var tarefaId = Guid.NewGuid();

        var request = new AtualizarTarefaRequest(
            "Estudar DDD",
            "Estudar arquitetura de software",
            PrioridadeTarefa.Media);

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/tarefas/{tarefaId}",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve concluir tarefa pela API")]
    public async Task Deve_Concluir_Tarefa_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var criacaoResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

        var criacaoResultado = await criacaoResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        var tarefaId = criacaoResultado!.TarefaId;

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/concluir",
            null);

        var resultado = await response.Content
            .ReadFromJsonAsync<ConcluirTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefaId);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Status.Should().Be(StatusTarefa.Concluida);
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        resultado.DataConclusao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve retornar NotFound ao concluir tarefa inexistente")]
    public async Task Deve_Retornar_NotFound_Ao_Concluir_Tarefa_Inexistente()
    {
        // Arrange
        var tarefaId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/concluir",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve cancelar tarefa pela API")]
    public async Task Deve_Cancelar_Tarefa_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var criacaoResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

        var criacaoResultado = await criacaoResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        var tarefaId = criacaoResultado!.TarefaId;

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/cancelar",
            null);

        var resultado = await response.Content
            .ReadFromJsonAsync<CancelarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefaId);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Status.Should().Be(StatusTarefa.Cancelada);
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);
    }


    [Fact(DisplayName = "Deve retornar NotFound ao cancelar tarefa inexistente")]
    public async Task Deve_Retornar_NotFound_Ao_Cancelar_Tarefa_Inexistente()
    {
        // Arrange
        var tarefaId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/cancelar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve iniciar tarefa pela API")]
    public async Task Deve_Iniciar_Tarefa_Pela_API()
    {
        // Arrange
        var request = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var criacaoResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            request);

        var criacaoResultado = await criacaoResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        var tarefaId = criacaoResultado!.TarefaId;

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/iniciar",
            null);

        var resultado = await response.Content
            .ReadFromJsonAsync<IniciarTarefaResultDto>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefaId);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Status.Should().Be(StatusTarefa.EmAndamento);
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);
    }


    [Fact(DisplayName = "Deve retornar NotFound ao iniciar tarefa inexistente")]
    public async Task Deve_Retornar_NotFound_Ao_Iniciar_Tarefa_Inexistente()
    {
        // Arrange
        var tarefaId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/tarefas/{tarefaId}/iniciar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Deve atribuir um usuário ativo a uma tarefa")]
    public async Task AtribuirUsuario_DeveRetornarOk_QuandoTarefaEUsuarioExistirem()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest,
            _jsonOptions);

        var tarefaContent = await tarefaResponse.Content.ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "danielmodestop@hotmail.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest,
            _jsonOptions);

        var usuarioContent = await usuarioResponse.Content.ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Arrange - Preparar requisição de atribuição
        var request = new AtribuirUsuarioTarefaRequest(usuarioContent!.UsuarioId);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefaContent!.TarefaId}/atribuir-usuario",
            request,
            _jsonOptions);

        var content = await response.Content.ReadFromJsonAsync<AtribuirUsuarioResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        content.Should().NotBeNull();
        content!.TarefaId.Should().Be(tarefaContent.TarefaId);
        content.UsuarioId.Should().Be(usuarioContent.UsuarioId);
        content.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve atribuir usuário quando a tarefa não existir")]
    public async Task AtribuirUsuario_DeveRetornarNotFound_QuandoTarefaNaoExistir()
    {
        // Arrange
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var request = new AtribuirUsuarioTarefaRequest(usuario!.UsuarioId);

        var tarefaIdInexistente = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefaIdInexistente}/atribuir-usuario",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Não deve atribuir usuário quando o usuário não existir")]
    public async Task AtribuirUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        var usuarioIdInexistente = Guid.NewGuid();

        var request = new AtribuirUsuarioTarefaRequest(usuarioIdInexistente);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/atribuir-usuario",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Não deve atribuir usuário quando estiver inativo")]
    public async Task AtribuirUsuario_DeveRetornarNotFound_QuandoUsuarioEstiverInativo()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Arrange - Desativar usuário
        var desativacaoResponse = await _client.PatchAsync(
            $"/api/usuarios/{usuario!.UsuarioId}/desativar",
            null);

        desativacaoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new AtribuirUsuarioTarefaRequest(usuario.UsuarioId);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/atribuir-usuario",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Não deve atribuir usuário a uma tarefa cancelada")]
    public async Task AtribuirUsuario_DeveRetornarUnprocessableEntity_QuandoTarefaEstiverCancelada()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Cancelar tarefa
        var cancelamentoResponse = await _client.PatchAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/cancelar",
            null);

        cancelamentoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var request = new AtribuirUsuarioTarefaRequest(usuario!.UsuarioId);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa.TarefaId}/atribuir-usuario",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Não deve atribuir usuário a uma tarefa concluída")]
    public async Task AtribuirUsuario_DeveRetornarUnprocessableEntity_QuandoTarefaEstiverConcluida()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Concluir tarefa
        var conclusaoResponse = await _client.PatchAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/concluir",
            null);

        conclusaoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var request = new AtribuirUsuarioTarefaRequest(usuario!.UsuarioId);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa.TarefaId}/atribuir-usuario",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact(DisplayName = "Deve atribuir usuário a uma tarefa em andamento")]
    public async Task AtribuirUsuario_DeveRetornarOk_QuandoTarefaEstiverEmAndamento()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Iniciar tarefa
        var iniciarResponse = await _client.PatchAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/iniciar",
            null);

        iniciarResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var request = new AtribuirUsuarioTarefaRequest(usuario!.UsuarioId);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa.TarefaId}/atribuir-usuario",
            request);

        var resultado = await response.Content
            .ReadFromJsonAsync<AtribuirUsuarioResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefa.TarefaId);
        resultado.UsuarioId.Should().Be(usuario.UsuarioId);
        resultado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve manter a data de atualização ao atribuir novamente o mesmo usuário")]
    public async Task AtribuirUsuario_NaoDeveAlterarData_QuandoMesmoUsuarioForAtribuidoNovamente()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Criar usuário
        var usuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            usuarioRequest);

        var usuario = await usuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var request = new AtribuirUsuarioTarefaRequest(usuario!.UsuarioId);

        // Act - Primeira atribuição
        var primeiraResponse = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/atribuir-usuario",
            request);

        var primeiraResultado = await primeiraResponse.Content
            .ReadFromJsonAsync<AtribuirUsuarioResultDto>(_jsonOptions);

        // Act - Segunda atribuição do mesmo usuário
        var segundaResponse = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa.TarefaId}/atribuir-usuario",
            request);

        var segundaResultado = await segundaResponse.Content
            .ReadFromJsonAsync<AtribuirUsuarioResultDto>(_jsonOptions);

        // Assert
        primeiraResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        segundaResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        primeiraResultado.Should().NotBeNull();
        segundaResultado.Should().NotBeNull();

        segundaResultado!.TarefaId.Should().Be(tarefa.TarefaId);
        segundaResultado.UsuarioId.Should().Be(usuario.UsuarioId);

        segundaResultado.DataAtualizacao.Should()
            .Be(primeiraResultado!.DataAtualizacao);
    }

    [Fact(DisplayName = "Deve substituir o usuário atribuído à tarefa")]
    public async Task AtribuirUsuario_DeveSubstituirUsuario_QuandoNovoUsuarioForInformado()
    {
        // Arrange - Criar tarefa
        var tarefaRequest = new CriarTarefaRequest(
            "Estudar C#",
            "Estudar testes de integração",
            PrioridadeTarefa.Alta);

        var tarefaResponse = await _client.PostAsJsonAsync(
            "/api/tarefas",
            tarefaRequest);

        var tarefa = await tarefaResponse.Content
            .ReadFromJsonAsync<CriarTarefaResultDto>(_jsonOptions);

        // Arrange - Criar primeiro usuário
        var primeiroUsuarioRequest = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var primeiroUsuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            primeiroUsuarioRequest);

        var primeiroUsuario = await primeiroUsuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Arrange - Criar segundo usuário
        var segundoUsuarioRequest = new CriarUsuarioRequest(
            "Maria Silva",
            "maria@email.com");

        var segundoUsuarioResponse = await _client.PostAsJsonAsync(
            "/api/usuarios",
            segundoUsuarioRequest);

        var segundoUsuario = await segundoUsuarioResponse.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Act - Primeira atribuição
        var primeiraRequest = new AtribuirUsuarioTarefaRequest(
            primeiroUsuario!.UsuarioId);

        var primeiraResponse = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa!.TarefaId}/atribuir-usuario",
            primeiraRequest);

        // Act - Substituir pelo segundo usuário
        var segundaRequest = new AtribuirUsuarioTarefaRequest(
            segundoUsuario!.UsuarioId);

        var segundaResponse = await _client.PatchAsJsonAsync(
            $"/api/tarefas/{tarefa.TarefaId}/atribuir-usuario",
            segundaRequest);

        var resultado = await segundaResponse.Content
            .ReadFromJsonAsync<AtribuirUsuarioResultDto>(_jsonOptions);

        // Assert
        primeiraResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        segundaResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.TarefaId.Should().Be(tarefa.TarefaId);
        resultado.UsuarioId.Should().Be(segundoUsuario.UsuarioId);
        resultado.UsuarioId.Should().NotBe(primeiroUsuario.UsuarioId);
    }
}
