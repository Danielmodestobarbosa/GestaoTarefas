

using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tarefas.Api.Requests.Usuarios;
using Tarefas.Api.Tests.Infrastructure;
using Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Domain.Usuarios.Enums;
using Xunit;

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

    // =========================================================
    // MÉTODOS AUXILIARES
    // =========================================================

    private static string GerarEmailUnico()
    {
        return $"teste_{Guid.NewGuid():N}@email.com";
    }

    private async Task<CriarUsuarioResultDto> CriarUsuarioAsync(
        string nome = "Daniel Modesto",
        string? email = null,
        string senha = "Senha@123")
    {
        email ??= GerarEmailUnico();

        var request = new CriarUsuarioRequest(
            nome,
            email,
            senha);

        var response = await _client.PostAsJsonAsync(
            "/api/usuarios",
            request,
            _jsonOptions);

        var conteudo = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            HttpStatusCode.Created,
            $"Falha ao criar usuário: {conteudo}");

        var resultado = JsonSerializer.Deserialize<CriarUsuarioResultDto>(
            conteudo,
            _jsonOptions);

        resultado.Should().NotBeNull();

        return resultado!;
    }

    private async Task AutenticarAsync(
        string email,
        string senha = "Senha@123")
    {
        var request = new AutenticarUsuarioRequest
        {
            Email = email,
            Senha = senha
        };

        var response = await _client.PostAsJsonAsync(
            "/api/usuarios/autenticar",
            request,
            _jsonOptions);

        var conteudo = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            $"Falha na autenticação: {conteudo}");

        var resultado =
            JsonSerializer.Deserialize<AutenticarUsuarioResultDto>(
                conteudo,
                _jsonOptions);

        resultado.Should().NotBeNull();
        resultado!.AccessToken.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                resultado.AccessToken);
    }

    private async Task<CriarUsuarioResultDto>
        CriarUsuarioEAutenticarAsync()
    {
        var email = GerarEmailUnico();

        var usuario = await CriarUsuarioAsync(
            email: email);

        await AutenticarAsync(email);

        return usuario;
    }

    // =========================================================
    // CRIAR USUÁRIO
    // =========================================================

    [Fact(DisplayName = "Deve cadastrar um usuário e retornar Created")]
    public async Task CriarUsuario_DeveRetornarCreated_QuandoDadosForemValidos()
    {
        // Arrange
        var email = GerarEmailUnico();

        var request = new CriarUsuarioRequest(
            "Daniel Modesto",
            email,
            "Senha@123");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/usuarios",
            request,
            _jsonOptions);

        var resultado = await response.Content
            .ReadFromJsonAsync<CriarUsuarioResultDto>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().NotBeEmpty();
        resultado.Nome.ToString().Should().Be("Daniel Modesto");
        resultado.Email.ToString().Should().Be(email);
        resultado.Status.Should().Be(StatusUsuario.Ativo);
    }

    // =========================================================
    // OBTER USUÁRIO POR ID
    // =========================================================

    [Fact(DisplayName = "Deve retornar o usuário quando encontrado pelo ID")]
    public async Task ObterUsuarioPorId_DeveRetornarOk_QuandoUsuarioExistir()
    {
        // Arrange
        var usuario = await CriarUsuarioEAutenticarAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/usuarios/{usuario.UsuarioId}");

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
        await CriarUsuarioEAutenticarAsync();

        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/usuarios/{usuarioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // =========================================================
    // LISTAR USUÁRIOS
    // =========================================================

    [Fact(DisplayName = "Deve retornar todos os usuários cadastrados")]
    public async Task ObterUsuarios_DeveRetornarTodosOsUsuarios()
    {
        // Arrange
        var email1 = GerarEmailUnico();
        var email2 = GerarEmailUnico();

        await CriarUsuarioAsync(
            "Daniel Modesto",
            email1);

        await CriarUsuarioAsync(
            "Maria Silva",
            email2);

        await AutenticarAsync(email1);

        // Act
        var response = await _client.GetAsync("/api/usuarios");

        var conteudo = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            $"Erro ao listar usuários: {conteudo}");

        var resultado =
            JsonSerializer.Deserialize<PagedResult<ObterUsuariosResultDto>>(
                conteudo,
                _jsonOptions);

        // Assert
        resultado.Should().NotBeNull();

        resultado!.Items.Should().HaveCount(2);

        resultado.Items.Should().Contain(u =>
            u.Nome == "Daniel Modesto" &&
            u.Email == email1);

        resultado.Items.Should().Contain(u =>
            u.Nome == "Maria Silva" &&
            u.Email == email2);

        resultado.TotalCount.Should().Be(2);
        resultado.TotalPages.Should().Be(1);
    }

    // =========================================================
    // ATUALIZAR USUÁRIO
    // =========================================================

    [Fact(DisplayName = "Deve atualizar os dados de um usuário existente")]
    public async Task AtualizarUsuario_DeveRetornarOk_QuandoUsuarioExistir()
    {
        // Arrange
        var email = GerarEmailUnico();

        var usuarioCriado = await CriarUsuarioAsync(
            email: email);

        await AutenticarAsync(email);

        var atualizacao = new AtualizarUsuarioRequest(
            "Daniel Atualizado",
            "daniel.atualizado@email.com");

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/usuarios/{usuarioCriado.UsuarioId}",
            atualizacao,
            _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Consultar novamente o usuário
        var consulta = await _client.GetAsync(
            $"/api/usuarios/{usuarioCriado.UsuarioId}");

        consulta.StatusCode.Should().Be(HttpStatusCode.OK);

        var usuarioAtualizado = await consulta.Content
            .ReadFromJsonAsync<ObterUsuarioPorIdResultDto>(_jsonOptions);

        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.Nome.ToString().Should().Be("Daniel Atualizado");
        usuarioAtualizado.Email.ToString()
            .Should().Be("daniel.atualizado@email.com");
    }

    [Fact(DisplayName = "Deve retornar NotFound ao atualizar um usuário inexistente")]
    public async Task AtualizarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync();

        var usuarioId = Guid.NewGuid();

        var request = new AtualizarUsuarioRequest(
            "Daniel Atualizado",
            "daniel.atualizado@email.com");

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/usuarios/{usuarioId}",
            request,
            _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // =========================================================
    // DESATIVAR USUÁRIO
    // =========================================================

    [Fact(DisplayName = "Deve desativar um usuário ativo")]
    public async Task DesativarUsuario_DeveRetornarOk_QuandoUsuarioEstiverAtivo()
    {
        // Arrange
        var usuario = await CriarUsuarioEAutenticarAsync();

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuario.UsuarioId}/desativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content
            .ReadFromJsonAsync<DesativarUsuarioResultDto>(_jsonOptions);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(usuario.UsuarioId);
        resultado.Status.Should().Be(StatusUsuario.Inativo);
    }

    [Fact(DisplayName = "Deve retornar NotFound ao desativar um usuário inexistente")]
    public async Task DesativarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync();

        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioId}/desativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // =========================================================
    // ATIVAR USUÁRIO
    // =========================================================

    [Fact(DisplayName = "Deve ativar um usuário anteriormente desativado")]
    public async Task AtivarUsuario_DeveRetornarOk_QuandoUsuarioEstiverInativo()
    {
        // Arrange
        var usuario = await CriarUsuarioEAutenticarAsync();

        var desativacaoResponse = await _client.PatchAsync(
            $"/api/usuarios/{usuario.UsuarioId}/desativar",
            null);

        desativacaoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuario.UsuarioId}/ativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content
            .ReadFromJsonAsync<AtivarUsuarioResultDto>(_jsonOptions);

        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(usuario.UsuarioId);
        resultado.Status.Should().Be(StatusUsuario.Ativo);
    }

    [Fact(DisplayName = "Deve retornar NotFound ao ativar um usuário inexistente")]
    public async Task AtivarUsuario_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync();

        var usuarioId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync(
            $"/api/usuarios/{usuarioId}/ativar",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // =========================================================
    // DESCARTAR RECURSOS
    // =========================================================

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
