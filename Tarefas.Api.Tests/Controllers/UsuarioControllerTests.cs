
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tarefas.Api.Requests.Usuarios;
using Tarefas.Api.Tests.Infrastructure;
using Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Domain.Usuarios.Enums;

namespace Tarefas.Api.Tests.Usuarios;

public sealed class UsuariosControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

    public UsuariosControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact(DisplayName = "Deve cadastrar um usuário e retornar Created")]
    public async Task CriarUsuario_DeveRetornarCreated_QuandoDadosForemValidos()
    {
        // Arrange
        var request = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/usuarios",
            request);

        var resultado = await response.Content.ReadFromJsonAsync<CriarUsuarioResultDto>(
            _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().NotBeEmpty();
        resultado.Nome.ToString().Should().Be("Daniel Modesto");
        resultado.Email.ToString().Should().Be("daniel@email.com");
        resultado.Status.Should().Be(Tarefas.Domain.Usuarios.Enums.StatusUsuario.Ativo);
    }


    [Fact(DisplayName = "Deve retornar o usuário quando encontrado pelo ID")]
    public async Task ObterUsuarioPorId_DeveRetornarOk_QuandoUsuarioExistir()
    {
        // Arrange
        var request = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var respostaCadastro = await _client.PostAsJsonAsync(
            "/api/usuarios",
            request);

        var usuarioCriado = await respostaCadastro.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Act
        var response = await _client.GetAsync(
            $"/api/usuarios/{usuarioCriado!.UsuarioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content
            .ReadFromJsonAsync<ObterUsuarioPorIdResultDto>(_jsonOptions);

        resultado.Should().NotBeNull();
    }


    [Fact(DisplayName = "Deve retornar NotFound quando o usuário não existir")]
    public async Task ObterUsuarioPorId_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/usuarios/{usuarioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve retornar todos os usuários cadastrados")]
    public async Task ObterUsuarios_DeveRetornarTodosOsUsuarios()
    {
        // Arrange
        var usuario1 = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var usuario2 = new CriarUsuarioRequest(
            "Maria Silva",
            "maria@email.com");

        await _client.PostAsJsonAsync("/api/usuarios", usuario1);
        await _client.PostAsJsonAsync("/api/usuarios", usuario2);

        // Act
        var response = await _client.GetAsync("/api/usuarios");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content
            .ReadFromJsonAsync<List<ObterUsuarioPorIdResultDto>>(_jsonOptions);

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
    }



    [Fact(DisplayName = "Deve atualizar os dados de um usuário existente")]
    public async Task AtualizarUsuario_DeveRetornarOk_QuandoUsuarioExistir()
    {
        // Arrange
        var cadastro = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var respostaCadastro = await _client.PostAsJsonAsync(
            "/api/usuarios",
            cadastro);

        var usuarioCriado = await respostaCadastro.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        var atualizacao = new AtualizarUsuarioRequest(
            "Daniel Atualizado",
            "daniel.atualizado@email.com");

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/usuarios/{usuarioCriado!.UsuarioId}",
            atualizacao);

        // Consulta novamente o usuário
        var consulta = await _client.GetAsync(
            $"/api/usuarios/{usuarioCriado.UsuarioId}");

        var usuarioAtualizado = await consulta.Content
            .ReadFromJsonAsync<ObterUsuarioPorIdResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        consulta.StatusCode.Should().Be(HttpStatusCode.OK);

        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.Nome.ToString().Should().Be("Daniel Atualizado");
        usuarioAtualizado.Email.ToString().Should().Be("daniel.atualizado@email.com");
    }


    [Fact(DisplayName = "Deve retornar NotFound ao atualizar um usuário inexistente")]
    public async Task AtualizarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var request = new AtualizarUsuarioRequest(
            "Daniel Atualizado",
            "daniel.atualizado@email.com");

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/usuarios/{usuarioId}",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve desativar um usuário ativo")]
    public async Task DesativarUsuario_DeveRetornarOk_QuandoUsuarioEstiverAtivo()
    {
        // Arrange
        var cadastro = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var respostaCadastro = await _client.PostAsJsonAsync(
            "/api/usuarios",
            cadastro);

        var usuarioCriado = await respostaCadastro.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioCriado!.UsuarioId}/desativar",
            null);

        var resultado = await response.Content
            .ReadFromJsonAsync<DesativarUsuarioResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(usuarioCriado.UsuarioId);
        resultado.Status.Should().Be(StatusUsuario.Inativo);
    }


    [Fact(DisplayName = "Deve retornar NotFound ao desativar um usuário inexistente")]
    public async Task DesativarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioId}/desativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact(DisplayName = "Deve ativar um usuário anteriormente desativado")]
    public async Task AtivarUsuario_DeveRetornarOk_QuandoUsuarioEstiverInativo()
    {
        // Arrange
        var cadastro = new CriarUsuarioRequest(
            "Daniel Modesto",
            "daniel@email.com");

        var respostaCadastro = await _client.PostAsJsonAsync(
            "/api/usuarios",
            cadastro);

        var usuarioCriado = await respostaCadastro.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        await _client.PatchAsync(
            $"/api/usuarios/{usuarioCriado!.UsuarioId}/desativar",
            null);

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioCriado.UsuarioId}/ativar",
            null);

        var resultado = await response.Content
            .ReadFromJsonAsync<AtivarUsuarioResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(usuarioCriado.UsuarioId);
        resultado.Status.Should().Be(Tarefas.Domain.Usuarios.Enums.StatusUsuario.Ativo);
    }


    [Fact(DisplayName = "Deve retornar NotFound ao ativar um usuário inexistente")]
    public async Task AtivarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioId}/ativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
