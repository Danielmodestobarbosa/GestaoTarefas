using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;
using Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

namespace Tarefas.Application.Tests.Queries.Tarefas.ObterTarefaPorId;

public sealed class ObterTarefaPorIdQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar a tarefa quando encontrada")]
    public async Task Deve_Retornar_Tarefa_Quando_Encontrada()
    {
        var tarefaId = Guid.NewGuid();

        //Arrange
        var tarefaEsperada = new ObterTarefaPorIdResultDto
        {
            Id = tarefaId,
            Titulo = "Estudar C#",
            Descricao = "Estudar testes unitários",
            Status = "Pendente",
            Prioridade = "Alta"
        };

        var repositorio = new FakeTarefaQueryRepository(tarefaEsperada);
        var handler = new ObterTarefaPorIdQueryHandler(repositorio);
        var query = new ObterTarefaPorIdQuery(tarefaId);

        //Act
        var resultado = await handler.HandleAsync(query);

        //Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(tarefaEsperada);

    }
}

internal sealed class FakeTarefaQueryRepository : ITarefaQueryRepository
{
    private readonly ObterTarefaPorIdResultDto? _tarefa;

    public FakeTarefaQueryRepository(ObterTarefaPorIdResultDto? tarefa)
    {
        _tarefa = tarefa;
    }

    public Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default)
    {
        return Task.FromResult(_tarefa);
    }

    public Task<IReadOnlyList<TarefaResumoDto>> ListarAsync(
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

