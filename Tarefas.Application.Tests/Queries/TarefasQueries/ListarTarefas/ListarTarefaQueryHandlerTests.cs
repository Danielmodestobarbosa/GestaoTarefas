

using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Tarefas.DTOs;
using Tarefas.Application.Queries.Tarefas.ListarTarefas;

namespace Tarefas.Application.Tests.Queries.Tarefas.ListarTarefas;

public sealed class ListarTarefaQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar a lista de tarefas paginada")]
    public async Task Deve_Retornar_Lista_De_Tarefas_Paginada()
    {
        // Arrange
        IReadOnlyList<TarefaResumoDto> tarefasEsperadas =
        [
            new TarefaResumoDto
            {
                TarefaId = Guid.NewGuid(),
                Titulo = "Estudar C#",
                Status = "Pendente",
                Prioridade = "Alta"
            },
            new TarefaResumoDto
            {
                TarefaId = Guid.NewGuid(),
                Titulo = "Estudar testes unitários",
                Status = "EmAndamento",
                Prioridade = "Media"
            }
        ];

        var repositorio = new FakeTarefaRepository(tarefasEsperadas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Items.Should().HaveCount(2);
        resultado.Items.Should().BeEquivalentTo(tarefasEsperadas);

        resultado.PageNumber.Should().Be(1);
        resultado.PageSize.Should().Be(10);
        resultado.TotalCount.Should().Be(2);
        resultado.TotalPages.Should().Be(1);
    }

    [Fact(DisplayName = "Deve retornar resultado vazio quando não houver tarefas")]
    public async Task Deve_Retornar_Resultado_Vazio_Quando_Nao_Houver_Tarefas()
    {
        // Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Items.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
        resultado.TotalPages.Should().Be(0);
    }

    [Fact(DisplayName = "Deve consultar o repositorio uma única vez")]
    public async Task Deve_Consultar_Repositorio_Uma_Unica_Vez()
    {
        // Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        repositorio.QuantidadeDeConsultas.Should().Be(1);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositorio")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        // Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await handler.Handle(query, token);

        // Assert
        repositorio.UltimoTokenRecebido.Should().Be(token);
    }

    [Fact(DisplayName = "Deve encaminhar os parâmetros de paginação ao repositorio")]
    public async Task Deve_Encaminhar_Parametros_De_Paginacao_Ao_Repositorio()
    {
        // Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);

        var query = new ListarTarefasQuery(
            PageNumber: 2,
            PageSize: 5);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        repositorio.UltimaPaginaRecebida.Should().Be(2);
        repositorio.UltimoTamanhoPaginaRecebido.Should().Be(5);
    }
}

internal sealed class FakeTarefaRepository : ITarefaQueryRepository
{
    public int QuantidadeDeConsultas { get; private set; }

    public CancellationToken UltimoTokenRecebido { get; private set; }

    public int UltimaPaginaRecebida { get; private set; }

    public int UltimoTamanhoPaginaRecebido { get; private set; }

    private readonly IReadOnlyList<TarefaResumoDto> _tarefas;

    public FakeTarefaRepository(IReadOnlyList<TarefaResumoDto> tarefas)
    {
        _tarefas = tarefas;
    }

    public Task<PagedResult<TarefaResumoDto>> ListarAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        QuantidadeDeConsultas++;

        UltimoTokenRecebido = ct;
        UltimaPaginaRecebida = pageNumber;
        UltimoTamanhoPaginaRecebido = pageSize;

        var resultado = new PagedResult<TarefaResumoDto>(
            _tarefas.ToList(),
            pageNumber,
            pageSize,
            _tarefas.Count);

        return Task.FromResult(resultado);
    }

    public Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
