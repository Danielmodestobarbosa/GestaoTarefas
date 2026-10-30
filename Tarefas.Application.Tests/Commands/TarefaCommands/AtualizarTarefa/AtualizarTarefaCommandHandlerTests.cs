using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Tests.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaCommandHandlerTests
{
    #region Testes de sucesso

    [Fact(DisplayName = "Deve atualizar uma tarefa quando encontrada")]
    public async Task Deve_Atualizar_Tarefa_Quando_Encontrada()
    {
        var tarefa = new Tarefa(
            "Titulo antigo",
            "Descricao antiga",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            tarefa.Id,
            "Novo titulo",
            "Nova descricao",
            PrioridadeTarefa.Media);

        //Act
        var resultado = await handler.Handle(command);

        //Assert
        resultado.TarefaId.Should().Be(tarefa.Id);
        resultado.Titulo.Should().Be("Novo titulo");
        resultado.Descricao.Should().Be("Nova descricao");
        resultado.Prioridade.Should().Be(PrioridadeTarefa.Media);

        repositorio.QuantidadeDeAtualizacoes.Should().Be(1);
    }

    #endregion

    #region Testes Cenário de erro

    [Fact(DisplayName = "Deve lançar erro quando a tarefa não for encontrada")]
    public async Task Deve_Retornar_Erro_Quando_Tarefa_Nao_Encontrada()
    {
        var tarefa = new Tarefa(
            "Titulo antigo",
            "Descricao antiga",
            PrioridadeTarefa.Alta);

        var repositorio = new FakeTarefaRepository(tarefa);
        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            Guid.NewGuid(),
            "Novo titulo",
            "Nova descricao",
            PrioridadeTarefa.Media);

        //Act
        var acao = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarefa não encontrada");

        //Já valida que o repositório não será atualizado
        repositorio.QuantidadeDeAtualizacoes.Should().Be(0);
    }


    [Fact(DisplayName = "Deve propagar exceção quando a atualização falhar")]
    public async Task Deve_Propagar_Excecao_Quando_Atualizacao_Falhar()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Título antigo",
            "Descrição antiga",
            PrioridadeTarefa.Baixa);

        var repositorio = new FakeTarefaRepository(tarefa)
        {
            DeveFalharAoAtualizar = true
        };

        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            tarefa.Id,
            "Novo título",
            "Nova descrição",
            PrioridadeTarefa.Alta);

        // Act
        var act = async () => await handler.Handle(command);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar tarefa");
    }

    [Fact(DisplayName = "Deve propagar exceção quando a consulta falhar")]
    public async Task Deve_Propagar_Excecao_Quando_Consulta_Falhar()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Título antigo",
            "Descrição antiga",
            PrioridadeTarefa.Baixa);

        var repositorio = new FakeTarefaRepository(tarefa)
        {
            DeveFalharAoConsultar = true
        };

        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            tarefa.Id,
            "Novo título",
            "Nova descrição",
            PrioridadeTarefa.Alta);

        // Act
        var act = async () => await handler.Handle(command);

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
        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            tarefa.Id,
            "Novo título",
            "Nova descrição",
            PrioridadeTarefa.Media);

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        await handler.Handle(command, cancellationToken);

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
        var handler = new AtualizarTarefaCommandHandler(repositorio);

        var command = new AtualizarTarefaCommand(
            tarefa.Id,
            "Novo título",
            "Nova descrição",
            PrioridadeTarefa.Media);

        // Act
        await handler.Handle(command);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(tarefa.Id);
    }

    #endregion
}

internal class FakeTarefaRepository : ITarefaRepository
{
    private readonly Tarefa _tarefa;

    public bool DeveFalharAoAtualizar { get; set; }
    public bool DeveFalharAoConsultar { get; set; }
    public CancellationToken UltimoTokenConsulta { get; private set; }
    public CancellationToken UltimoTokenAtualizacao { get; private set; }
    public int QuantidadeDeAtualizacoes { get; private set; }
    public Guid UltimoIdConsultado { get; private set; }

    public FakeTarefaRepository(Tarefa tarefa)
    {
        _tarefa = tarefa;
    }

    public Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId, CancellationToken
        cancellationToken = default)
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

    public Task AtualizarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default)
    {
        UltimoTokenAtualizacao = cancellationToken;

        if (DeveFalharAoAtualizar)
        {
            throw new InvalidOperationException(
                "Falha simulada ao atualizar tarefa");
        }

        QuantidadeDeAtualizacoes++;
        return Task.CompletedTask;
    }

}

