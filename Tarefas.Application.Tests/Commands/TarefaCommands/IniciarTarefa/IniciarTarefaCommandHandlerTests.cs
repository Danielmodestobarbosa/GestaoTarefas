
using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;
using Tarefas.Application.Tests.Commands.FakesUnitOfWork;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Tests.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommandHandlerTests
{
    [Fact(DisplayName = "Deve iniciar tarefa quando encontrada")]
    public async Task Deve_Iniciar_Tarefa_Quando_Encontrada()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new IniciarTarefaCommandHandler(
            repositorio, 
            unitOfWork);

        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.TarefaId.Should().Be(tarefa.Id);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Status.Should().Be(StatusTarefa.EmAndamento);
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando a tarefa não for encontrada")]
    public async Task Deve_Lancar_Excecao_Quando_Tarefa_Nao_For_Encontrada()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new IniciarTarefaCommandHandler(
            repositorio, 
            unitOfWork);
        var command = new IniciarTarefaCommand(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarefa não encontrada");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve propagar exceção quando falhar ao atualizar")]
    public async Task Deve_Propagar_Excecao_Quando_Falhar_Ao_Atualizar()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var unitOfWork = new FakeUnitOfWork
        {
            ExcecaoAoCommit = new InvalidOperationException(
                "Falha simulada ao atualizar tarefa")
        };

        var handler = new IniciarTarefaCommandHandler(
            repositorio, 
            unitOfWork);

        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar tarefa");

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }

    [Fact(DisplayName = "Deve propagar exceção quando a consulta no repositório falhar")]
    public async Task Deve_Propagar_Excecao_Quando_Consulta_No_Repositorio_Falhar()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa)
        {
            DeveFalharAoConsultar = true
        };

        var unitOfWork = new FakeUnitOfWork();
        var handler = new IniciarTarefaCommandHandler(
            repositorio,
            unitOfWork);
        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao consultar tarefa");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositório")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new IniciarTarefaCommandHandler(
            repositorio,
            unitOfWork);

        var command = new IniciarTarefaCommand(tarefa.Id);

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenConsulta.Should().Be(cancellationToken);
        unitOfWork.UltimoTokenCommit.Should().Be(cancellationToken);
    }

    [Fact(DisplayName = "Deve consultar usando o ID correto da tarefa")]
    public async Task Deve_Consultar_Usando_Id_Correto_Da_Tarefa()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new IniciarTarefaCommandHandler(
            repositorio,
            unitOfWork);

        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(tarefa.Id);
    }
}

internal class FakeTarefaRepository : ITarefaRepository
{
    private readonly Tarefa _tarefa;

    public bool DeveFalharAoConsultar { get; set; }

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public Guid UltimoIdConsultado { get; private set; }

    public FakeTarefaRepository(Tarefa tarefa)
    {
        _tarefa = tarefa;
    }

    public Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;

        UltimoIdConsultado = tarefaId;

        if (DeveFalharAoConsultar)
        {
            throw new InvalidOperationException(
                "Falha simulada ao consultar tarefa");
        }

        return Task.FromResult<Tarefa?>(
            tarefaId == _tarefa.Id ? _tarefa : null);
    }

    public Task AdicionarAsync(
        Tarefa tarefa,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}


