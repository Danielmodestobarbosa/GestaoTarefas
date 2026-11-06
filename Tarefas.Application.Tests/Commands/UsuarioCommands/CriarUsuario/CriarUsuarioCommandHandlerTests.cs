
using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Tests.Commands.FakesUnitOfWork;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.UsuarioCommands.CriarUsuario;

public sealed class CriarUsuarioCommandHandlerTests
{
    #region Testes de Criação do usuário

    [Fact(DisplayName = "Deve criar o usuário com sucesso")]
    public async Task HandleAsync_DeveCriarUsuario_QuandoCommandForValido()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().NotBeEmpty();
        resultado.Nome.Should().Be(nome);
        resultado.Email.Should().Be(email);
        resultado.Status.Should().Be(StatusUsuario.Ativo);

        repositorio.UsuarioAdicionado.Should().NotBeNull();
        repositorio.UsuarioAdicionado!.Id.Should().Be(resultado.UsuarioId);
        repositorio.QuantidadeDeAdicoes.Should().Be(1);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve adicionar o usuário apenas uma vez")]
    public async Task HandleAsync_DeveAdicionarUsuarioApenasUmaVez()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        await handler.Handle(command);

        // Assert
        repositorio.QuantidadeDeAdicoes.Should().Be(1);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve repassar o CancellationToken ao repositório")]
    public async Task HandleAsync_DeveRepassarCancellationToken()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenAdicao.Should().Be(cancellationToken);
    }


    [Fact(DisplayName = "Deve adicionar ao repositório o mesmo usuário retornado")]
    public async Task HandleAsync_DeveAdicionarMesmoUsuarioRetornado()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        repositorio.UsuarioAdicionado.Should().NotBeNull();

        repositorio.UsuarioAdicionado!.Id.Should().Be(resultado.UsuarioId);
        repositorio.UsuarioAdicionado.Nome.Should().Be(resultado.Nome);
        repositorio.UsuarioAdicionado.Email.Should().Be(resultado.Email);
        repositorio.UsuarioAdicionado.Status.Should().Be(resultado.Status);
    }


    [Fact(DisplayName = "Deve lançar exceção quando falhar ao adicionar usuário")]
    public async Task HandleAsync_DeveLancarExcecao_QuandoFalharAoAdicionar()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository
        {
            DeveFalharAoAdicionar = true
        };

        var unitOfWork = new FakeUnitOfWork();
        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao adicionar usuário");

        repositorio.QuantidadeDeAdicoes.Should().Be(1);
        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve preservar nome e e-mail informados no comando")]
    public async Task HandleAsync_DevePreservarDadosDoCommand()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.Nome.Should().Be(nome);
        resultado.Email.Should().Be(email);

        repositorio.UsuarioAdicionado!.Nome.Should().Be(nome);
        repositorio.UsuarioAdicionado.Email.Should().Be(email);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve criar o usuário com status Ativo")]
    public async Task HandleAsync_DeveCriarUsuarioComStatusAtivo()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.Status.Should().Be(StatusUsuario.Ativo);
        repositorio.UsuarioAdicionado!.Status.Should().Be(StatusUsuario.Ativo);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve criar o usuário com identificador válido")]
    public async Task HandleAsync_DeveGerarIdentificadorValido()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.UsuarioId.Should().NotBe(Guid.Empty);
        repositorio.UsuarioAdicionado!.Id.Should().NotBe(Guid.Empty);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Não deve retornar resultado quando falhar ao adicionar usuário")]
    public async Task HandleAsync_NaoDeveRetornarResultado_QuandoFalharAoAdicionar()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var command = new CriarUsuarioCommand(nome, email);

        var repositorio = new FakeUsuarioRepository
        {
            DeveFalharAoAdicionar = true
        };

        var unitOfWork = new FakeUnitOfWork();
        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>();

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    #endregion
}

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario? _usuario;

    public Usuario? UsuarioAdicionado { get; private set; }

    public int QuantidadeDeAdicoes { get; private set; }

    public bool DeveFalharAoAdicionar { get; set; }

    public bool DeveFalharAoConsultar { get; set; }

    public CancellationToken UltimoTokenAdicao { get; private set; }

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public Guid UltimoIdConsultado { get; private set; }

    public FakeUsuarioRepository(Usuario? usuario  = null)
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
            _usuario?.Id == usuarioId ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenAdicao = cancellationToken;

        QuantidadeDeAdicoes++;
        UsuarioAdicionado = usuario;

        if (DeveFalharAoAdicionar)
        {
            throw new InvalidOperationException(
                "Falha simulada ao adicionar usuário");
        }

        return Task.CompletedTask;
    }
}

