using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Application.Queries.Usuarios.ObterUsuarios;

namespace Tarefas.Application.Tests.Queries.Usuarios.ObterUsuarios;

public sealed class ObterUsuariosQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar todos os usuários cadastrados")]
    public async Task Handle_DeveRetornarTodosUsuarios()
    {
        // Arrange
        var usuariosEsperados = new List<ObterUsuariosResultDto>
        {
            new(Guid.NewGuid(), "Daniel Modesto", "danielmodesto@hotmail.com", "Ativo"),
            new(Guid.NewGuid(), "Maria Silva", "maria@email.com", "Ativo")
        };

        var repository = new FakeUsuarioQueryRepository(usuariosEsperados);

        var handler = new ObterUsuariosQueryHandler(repository);

        var query = new ObterUsuariosQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(usuariosEsperados);
    }

    [Fact(DisplayName = "Deve retornar lista vazia quando não houver usuários")]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremUsuarios()
    {
        // Arrange
        var repository = new FakeUsuarioQueryRepository(
            new List<ObterUsuariosResultDto>());

        var handler = new ObterUsuariosQueryHandler(repository);

        var query = new ObterUsuariosQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}

internal sealed class FakeUsuarioQueryRepository : IUsuarioQueryRepository
{
    private readonly IReadOnlyList<ObterUsuariosResultDto> _usuarios;

    public FakeUsuarioQueryRepository(
        IReadOnlyList<ObterUsuariosResultDto> usuarios)
    {
        _usuarios = usuarios;
    }

    public Task<IReadOnlyList<ObterUsuariosResultDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_usuarios);
    }

    public Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ObterUsuarioPorIdResultDto?>(null);
    }
}