using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Application.Queries.Usuarios.ObterUsuarios;

namespace Tarefas.Application.Tests.Queries.Usuarios.ObterUsuarios;

public sealed class ObterUsuariosQueryHandlerTests
{

    [Fact(DisplayName = "Deve retornar todos os usuários cadastrados")]
    public async Task Handle_DeveRetornarTodosUsuarios()
    {
        // Arrange
        IReadOnlyList<ObterUsuariosResultDto> usuariosEsperados =
        [
            new(Guid.NewGuid(), "Daniel Modesto", "danielmodesto@hotmail.com", "Ativo"),
        new(Guid.NewGuid(), "Maria Silva", "maria@email.com", "Ativo")
        ];

        var repository = new FakeUsuarioQueryRepository(usuariosEsperados);

        var handler = new ObterUsuariosQueryHandler(repository);

        var query = new ObterUsuariosQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Items.Should().HaveCount(2);
        resultado.Items.Should().BeEquivalentTo(usuariosEsperados);

        resultado.PageNumber.Should().Be(1);
        resultado.PageSize.Should().Be(10);
        resultado.TotalCount.Should().Be(2);
        resultado.TotalPages.Should().Be(1);
    }

    [Fact(DisplayName = "Deve retornar lista vazia quando não houver usuários")]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremUsuarios()
    {
        // Arrange
        IReadOnlyList<ObterUsuariosResultDto> usuarios = [];

        var repository = new FakeUsuarioQueryRepository(usuarios);

        var handler = new ObterUsuariosQueryHandler(repository);

        var query = new ObterUsuariosQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Items.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
        resultado.TotalPages.Should().Be(0);
    }



    internal sealed class FakeUsuarioQueryRepository : IUsuarioQueryRepository
    {
        private readonly IReadOnlyList<ObterUsuariosResultDto> _usuarios;

        public FakeUsuarioQueryRepository(
            IReadOnlyList<ObterUsuariosResultDto> usuarios)
        {
            _usuarios = usuarios;
        }

        public Task<PagedResult<ObterUsuariosResultDto>> ListarAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var usuariosOrdenados = _usuarios
                .OrderBy(usuario => usuario.Nome)
                .ThenBy(usuario => usuario.Id)
                .ToList();

            var totalCount = usuariosOrdenados.Count;

            var items = usuariosOrdenados
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var resultado = new PagedResult<ObterUsuariosResultDto>(
                items,
                pageNumber,
                pageSize,
                totalCount);

            return Task.FromResult(resultado);
        }

        public Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ObterUsuarioPorIdResultDto?>(null);
        }
    }
}
