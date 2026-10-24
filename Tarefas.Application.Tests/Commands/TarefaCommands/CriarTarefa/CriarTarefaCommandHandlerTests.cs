using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Tests.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaCommandHandlerTests
{
    #region Testes de criação da tarefaHandler

    [Fact(DisplayName = "Deve criar tarefa com sucesso")]
    public async Task Deve_Criar_Tarefa_Com_Sucesso()
    {
        //Arrange
        var repositorio = new FakeTarefaRepository();
        var handler = new CriarTarefaCommandHandler(repositorio);

        var command = new CriarTarefaCommand(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        // Act
        var resultado = await handler.HandleAsync(command);

        // Assert
        resultado.TarefaId.Should().NotBeEmpty();
        resultado.Titulo.Should().Be("Estudar C#");
        resultado.Descricao.Should().Be("Estudar testes unitários");
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Alta);

        repositorio.QuantidadeDeAdicoes.Should().Be(1);
        repositorio.UltimaTarefaAdicionada.Should().NotBeNull();
    }


    [Fact(DisplayName = "Deve propagar exceção quando adicionar tarefa no repositório falhar")]
    public async Task Deve_Propagar_Excecao_Quando_Adicionar_Tarefa_No_Repositorio_Falhar()
    {
        // Arrange
        var repositorio = new FakeTarefaRepository
        {
            DeveFalharAoAdicionar = true
        };

        var handler = new CriarTarefaCommandHandler(repositorio);

        var command = new CriarTarefaCommand(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        // Act
        var act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao adicionar tarefa");

        repositorio.QuantidadeDeAdicoes.Should().Be(1);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositório")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        // Arrange
        var repositorio = new FakeTarefaRepository();
        var handler = new CriarTarefaCommandHandler(repositorio);

        var command = new CriarTarefaCommand(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        await handler.HandleAsync(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenAdicao.Should()
            .Be(cancellationToken);
    }

    #endregion
}

internal sealed class FakeTarefaRepository : ITarefaRepository
{
    public Tarefa? UltimaTarefaAdicionada { get; private set; }
    public int QuantidadeDeAdicoes { get; private set; }
    public bool DeveFalharAoAdicionar { get; set; }
    public CancellationToken UltimoTokenAdicao { get; private set; }

    public Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AdicionarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default)
    {
        UltimoTokenAdicao = cancellationToken;

        QuantidadeDeAdicoes++;
        UltimaTarefaAdicionada = tarefa;

        if (DeveFalharAoAdicionar)
        {
            throw new InvalidOperationException(
                "Falha simulada ao adicionar tarefa");
        }

        return Task.CompletedTask;
    }

    public Task AtualizarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
