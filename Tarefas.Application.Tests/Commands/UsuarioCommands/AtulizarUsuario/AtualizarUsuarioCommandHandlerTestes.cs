using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;
using Tarefas.Application.Tests.Commands.FakesUnitOfWork;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.UsuarioCommands.AtulizarUsuario;

public sealed class AtualizarUsuarioCommandHandlerTestes
{
    #region Testes de Atualizar usuário

    [Fact(DisplayName = "Deve atualizar perfil quando usuário encontrado")]
    public async Task Deve_Atualizar_Perfil_Quando_Usuario_Encontrado()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var novoNome = new NomeUsuario("Daniel Barbosa");
        var novoEmail = new Email("daniel.barbosa@hotmail.com");

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            novoNome,
            novoEmail);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Nome.Should().Be("Daniel Barbosa");
        resultado.Email.Endereco.Should().Be("daniel.barbosa@hotmail.com");
        resultado.Status.Should().Be(StatusUsuario.Ativo);

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve lançar exceção quando usuário não for encontrado")]
    public async Task Deve_Lancar_Excecao_Quando_Usuario_Nao_Encontrado()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            Guid.NewGuid(),
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Usuario não encontrado");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve propagar exceção quando falhar ao atualizar")]
    public async Task Deve_Propagar_Excecao_Ao_Falhar_Ao_Atualizar()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork
        {
            ExcecaoAoCommit = new InvalidOperationException(
                "Falha simulada ao atualizar usuário")
        };

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar usuário");

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }

    [Fact(DisplayName = "Deve propagar exceção quando falhar ao consultar")]
    public async Task Deve_Propagar_Excecao_Ao_Falhar_Ao_Consultar()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var repositorio = new FakeUsuarioRepository(usuario)
        {
            DeveFalharAoConsultar = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao consultar usuário");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve lançar DomainException quando usuário estiver inativo")]
    public async Task Deve_Lancar_DomainException_Quando_Usuario_Inativo()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        usuario.DesativarPerfil();

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Usuários inativos não podem ter o perfil atualizado");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve encaminhar CancellationToken ao repositório")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenConsulta.Should().Be(cancellationToken);
        unitOfWork.UltimoTokenCommit.Should().Be(cancellationToken);
    }

    [Fact(DisplayName = "Deve consultar usando o ID correto do usuário")]
    public async Task Deve_Consultar_Usando_Id_Correto_Do_Usuario()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AtualizarUsuarioCommandHandler(
            repositorio,
            unitOfWork
            );

        var command = new AtualizarUsuarioCommand(
            usuario.Id,
            new NomeUsuario("Daniel Barbosa"),
            new Email("daniel.barbosa@hotmail.com"));

        // Act
        await handler.Handle(command);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(usuario.Id);
    }


    #endregion
}

internal class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario _usuario;

    public bool DeveFalharAoConsultar { get; set; }

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public Guid UltimoIdConsultado { get; private set; }

    public FakeUsuarioRepository(Usuario usuario)
    {
        _usuario = usuario;
    }

    public Task<Usuario?> ObterUsuarioPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;

        UltimoIdConsultado = usuarioId;

        if (DeveFalharAoConsultar)
        {
            throw new InvalidOperationException(
                "Falha simulada ao consultar usuário");
        }

        return Task.FromResult<Usuario?>(
            usuarioId == _usuario.Id ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

