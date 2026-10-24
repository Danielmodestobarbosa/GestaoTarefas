using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;
using Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Tests.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommandHandlerTests
{
    [Fact(DisplayName = "Deve iniciar tarefa quando encontrada")]
    public async Task Deve_Concluir_Tarefa_Quando_Encontrada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var handler = new IniciarTarefaCommandHandler(repositorio);

        var command = new IniciarTarefaCommand(tarefa.Id);

        //Act
        var resultado = await handler.HandleAsync(command);

        //Assert
        resultado.TarefaId.Should().Be(tarefa.Id);
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Status.Should().Be(StatusTarefa.EmAndamento);
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);

        repositorio.QuantidadeDeAtualizacoes.Should().Be(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando a tarefa não for encontrada")]
    public async Task Deve_Lançar_Exceção_Quando_Tarefa_Nao_For_Encontrada()
    {
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var handler = new IniciarTarefaCommandHandler(repositorio);

        var command = new IniciarTarefaCommand(Guid.NewGuid());

        //Act
        var acao = async () => await handler.HandleAsync(command);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarefa não encontrada");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(0);
    }

    [Fact(DisplayName = "Deve propagar exceção quando falhar ao atualizar")]
    public async Task Deve_Propagar_Excecao_Quando_Falhar_Ao_Atualizar()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa)
        {
            DeveFalharAoAtualizar = true
        };

        var handler = new IniciarTarefaCommandHandler(repositorio);
        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        var act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar tarefa");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(1);
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

        var handler = new IniciarTarefaCommandHandler(repositorio);
        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        var act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao consultar tarefa");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(0);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositório")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Título antigo",
            "Descrição antiga",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var handler = new IniciarTarefaCommandHandler(repositorio);

        var command = new IniciarTarefaCommand(tarefa.Id);

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        await handler.HandleAsync(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenConsulta.Should()
            .Be(cancellationToken);

        repositorio.UltimoTokenAtualizacao.Should()
            .Be(cancellationToken);
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
        var handler = new IniciarTarefaCommandHandler(repositorio);

        var command = new IniciarTarefaCommand(tarefa.Id);

        // Act
        await handler.HandleAsync(command);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(tarefa.Id);
    }
}

internal sealed class FakeTarefaRepository : ITarefaRepository
{
    private readonly Tarefa _tarefa;

    public FakeTarefaRepository(Tarefa tarefa)
    {
        _tarefa = tarefa;
    }

    public bool DeveFalharAoAtualizar { get; set; }
    public int QuantidadeDeAtualizacoes { get; private set; }
    public bool DeveFalharAoConsultar { get; set; }
    public CancellationToken UltimoTokenConsulta { get; private set; }
    public CancellationToken UltimoTokenAtualizacao { get; private set; }
    public Guid UltimoIdConsultado { get; private set; }

    public Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId, CancellationToken
        cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;

        if (DeveFalharAoConsultar)
            throw new InvalidOperationException(
                "Falha simulada ao consultar tarefa");

        return Task.FromResult<Tarefa?>(
            tarefaId == _tarefa.Id ? _tarefa : null);
    }

    public Task AdicionarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AtualizarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default)
    {
        UltimoIdConsultado = tarefa.Id;

        UltimoTokenAtualizacao = cancellationToken;

        QuantidadeDeAtualizacoes++;

        if (DeveFalharAoAtualizar)
            throw new InvalidOperationException(
                "Falha simulada ao atualizar tarefa");

        return Task.CompletedTask;
    }
}

