using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;
using Tarefas.Application.Tests.Fakes;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioTarefaCommandHandlerTests
{
    #region Teste de Atribuição de usuário

    [Fact(DisplayName = "Deve atribuir um usuário quando tarefa e usuario existirem e usuario estiver ativo")]
    public async Task Deve_Atribuir_Usuario_Quando_Tarefa_Usuario_Existirem_E_Usuario_Ativo()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email ("daniel@hotmail.com"));

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            tarefa.Id,
            usuario.Id);

        //Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        //Assert
        tarefa.UsuarioId.Should().Be(usuario.Id);
        resultado.TarefaId.Should().Be(tarefa.Id);
        resultado.UsuarioId.Should().Be(usuario.Id);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando a tarefa não for encontrada")]
    public async Task Deve_Lançar_Exceção_Quando_Tarefa_Não_For_Encontrada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            Guid.NewGuid(),
            usuario.Id);

        //Act
        var acao = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarefa não encontrada");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o usuário não for encontrado")]
    public async Task Deve_Lançar_Exceção_Quando_Usuario_Não_For_Encontrada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            tarefa.Id,
            Guid.NewGuid());

        //Act
        var acao = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Usuário não encontrado");

        tarefa.UsuarioId.Should().BeNull();
        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o usuário estivar inativo")]
    public async Task Deve_Lançar_Exceção_Quando_Usuario_Estiver_Inativo()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        usuario.DesativarPerfil();

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            tarefa.Id,
            usuario.Id);

        //Act
        var acao = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Não é possível atribuir uma tarefa a um usuário inativo");

        tarefa.UsuarioId.Should().BeNull();
        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve encaminhar o CancellationToken aos repositórios")]
    public async Task Deve_Encaminhar_CancellationToken_Aos_Repositorios()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            tarefa.Id,
            usuario.Id);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await handler.Handle(command, token);

        // Assert
        tarefaRepository.UltimoTokenConsulta.Should().Be(token);
        usuarioRepository.UltimoTokenConsulta.Should().Be(token);
        unitOfWork.UltimoTokenCommit.Should().Be(token);
    }

    [Fact(DisplayName = "Deve propagar a exceção quando ocorrer uma falha ao salvar a tarefa")]
    public async Task Deve_Propagar_Excecao_Quando_Falhar_Ao_Salvar_A_Tarefa()
    {
        // Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var tarefaRepository = new FakeTarefaCommandRepository(tarefa);
        var usuarioRepository = new FakeUsuarioRepository(usuario);

        var unitOfWork = new FakeUnitOfWork
        {
            ExcecaoAoCommit = new InvalidOperationException(
                "Falha ao salvar tarefa")
        };

        var handler = new AtribuirUsuarioTarefaCommandHandler(
            tarefaRepository,
            usuarioRepository,
            unitOfWork);

        var command = new AtribuirUsuarioTarefaCommand(
            tarefa.Id,
            usuario.Id);

        // Act
        var act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var excecao = await act.Should()
            .ThrowAsync<InvalidOperationException>();

        excecao.Which.Message.Should().Be("Falha ao salvar tarefa");

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }

    #endregion

}

internal sealed class FakeTarefaCommandRepository : ITarefaRepository
{
    private readonly Tarefa _tarefa;

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public FakeTarefaCommandRepository(Tarefa tarefa)
    {
        _tarefa = tarefa;
    }

    public Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;

        return Task.FromResult(
            _tarefa.Id == tarefaId ? _tarefa : null);
    }

    public Task AdicionarAsync(
        Tarefa tarefa,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}


internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario _usuario;

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public FakeUsuarioRepository(Usuario usuario)
    {
        _usuario = usuario;
    }

    public Task<Usuario?> ObterUsuarioPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;

        return Task.FromResult(
            _usuario.Id == usuarioId ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}


