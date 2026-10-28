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
        var resultado = await handler.Handle(query, CancellationToken.None);

        //Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(tarefaEsperada);
    }

    [Fact(DisplayName = "Deve retornar null quando a tarefa não for encontrado")]
    public async Task Deve_Retornar_Null_Quando_Tarefa_Nao_For_Encontrada()
    {
        //Arrange
        var tarefaId = Guid.NewGuid();

        var repositorio = new FakeTarefaQueryRepository(null);
        var handler = new ObterTarefaPorIdQueryHandler(repositorio);
        var query = new ObterTarefaPorIdQuery(tarefaId);

        //Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        //Assert
        resultado.Should().BeNull();
    }

    [Fact(DisplayName = "Deve consultar o repositório com o Id informado")]
    public async Task Deve_Consultar_Repositorio_Com_Id_Informado()
    {
        //Arrange
        var tarefaId = Guid.NewGuid();

        var repositorio = new FakeTarefaQueryRepository(null);
        var handler = new ObterTarefaPorIdQueryHandler(repositorio);
        var query = new ObterTarefaPorIdQuery(tarefaId);

        //Act 
        await handler.Handle(query, CancellationToken.None);

        //Assert
        repositorio.UltimoIdConsultado.Should().Be(tarefaId);

    }
}



internal sealed class FakeTarefaQueryRepository : ITarefaQueryRepository
{
    public Guid? UltimoIdConsultado { get; private set; }

    private readonly ObterTarefaPorIdResultDto? _tarefa;

    public FakeTarefaQueryRepository(ObterTarefaPorIdResultDto? tarefa)
    {
        _tarefa = tarefa;
    }

    public Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default)
    {
        UltimoIdConsultado = tarefaId;

        return Task.FromResult(_tarefa);
    }

    public Task<IReadOnlyList<TarefaResumoDto>> ListarAsync(
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

