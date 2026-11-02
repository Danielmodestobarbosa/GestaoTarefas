using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Application.Queries.Usuarios.ObterUsuarioPorId;

namespace Tarefas.Application.Tests.Queries.UsuariosQueries.ObterUsuarioPorId;

public sealed class ObterUsuarioPorIdQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar o usuário quando encontrado")]
    public async Task Handle_DeveRetornarUsuario_QuandoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var usuarioEsperado = new ObterUsuarioPorIdResultDto(
            usuarioId,
            "Daniel Modesto",
            "danielmodesto@hotmail.com",
            "Ativo");

        var repository = new FakeUsuarioQueryRepository(usuarioEsperado);

        var handler = new ObterUsuarioPorIdQueryHandler(repository);

        var query = new ObterUsuarioPorIdQuery(usuarioId);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(usuarioEsperado);
        repository.UltimoIdConsultado.Should().Be(usuarioId);
    }

    [Fact(DisplayName = "Deve retornar nulo quando o usuário não for encontrado")]
    public async Task Handle_DeveRetornarNulo_QuandoNaoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var repository = new FakeUsuarioQueryRepository(null);

        var handler = new ObterUsuarioPorIdQueryHandler(repository);

        var query = new ObterUsuarioPorIdQuery(usuarioId);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        repository.UltimoIdConsultado.Should().Be(usuarioId);
    }
}

internal sealed class FakeUsuarioQueryRepository : IUsuarioQueryRepository
{
    private readonly ObterUsuarioPorIdResultDto? _usuario;

    public Guid UltimoIdConsultado { get; private set; }

    public FakeUsuarioQueryRepository(ObterUsuarioPorIdResultDto? usuario)
    {
        _usuario = usuario;
    }

    public Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        UltimoIdConsultado = usuarioId;

        return Task.FromResult(
            _usuario?.Id == usuarioId ? _usuario : null);
    }

    public Task<IReadOnlyList<ObterUsuariosResultDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ObterUsuariosResultDto>>(
            Array.Empty<ObterUsuariosResultDto>());
    }
}