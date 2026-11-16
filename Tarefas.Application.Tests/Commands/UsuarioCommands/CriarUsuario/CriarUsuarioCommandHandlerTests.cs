
using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Tests.Commands.FakesUnitOfWork;
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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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

        passwordHasher.UltimaSenhaRecebida.Should().Be(senha);

        repositorio.UsuarioAdicionado!.PasswordHash
            .Should().Be(passwordHasher.HashGerado);

        repositorio.UsuarioAdicionado.PasswordHash
            .Should().NotBe(senha);
    }

    [Fact(DisplayName = "Deve utilizar a senha recebida no comando para gerar o hash")]
    public async Task HandleAsync_DeveUtilizarSenhaDoCommandParaGerarHash()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

        // Act
        await handler.Handle(command);

        // Assert
        passwordHasher.UltimaSenhaRecebida.Should().Be(senha);
        repositorio.UsuarioAdicionado!.PasswordHash
            .Should().Be(passwordHasher.HashGerado);

        repositorio.UsuarioAdicionado.PasswordHash
            .Should().NotBe(senha);
    }

    [Fact(DisplayName = "Deve adicionar o usuário apenas uma vez")]
    public async Task HandleAsync_DeveAdicionarUsuarioApenasUmaVez()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository
        {
            DeveFalharAoAdicionar = true
        };

        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();
        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

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
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository
        {
            DeveFalharAoAdicionar = true
        };

        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();
        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>();

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }

    [Fact(DisplayName = "Deve gerar hash antes de adicionar usuário")]
    public async Task HandleAsync_DeveGerarHashAntesDeAdicionarUsuario()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");
        const string senha = "Senha@123";

        var command = new CriarUsuarioCommand(nome, email, senha);

        var repositorio = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new FakePasswordHasher();

        var handler = new CriarUsuarioCommandHandler(
            repositorio,
            unitOfWork,
            passwordHasher);

        // Act
        await handler.Handle(command);

        // Assert
        passwordHasher.UltimaSenhaRecebida.Should().Be(senha);

        repositorio.UsuarioAdicionado.Should().NotBeNull();

        repositorio.UsuarioAdicionado!.PasswordHash
            .Should().Be(passwordHasher.HashGerado);

        repositorio.QuantidadeDeAdicoes.Should().Be(1);
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
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

    public string? UltimoEmailConsultado { get; private set; }

    public FakeUsuarioRepository(Usuario? usuario = null)
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
            throw new InvalidOperationException(
                "Falha simulada ao consultar usuário");

        return Task.FromResult<Usuario?>(
            _usuario?.Id == usuarioId ? _usuario : null);
    }

    public Task<Usuario?> ObterUsuarioPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenConsulta = cancellationToken;
        UltimoEmailConsultado = email;

        if (DeveFalharAoConsultar)
            throw new InvalidOperationException(
                "Falha simulada ao consultar usuário");

        return Task.FromResult<Usuario?>(
            _usuario?.Email.Endereco == email ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        UltimoTokenAdicao = cancellationToken;

        QuantidadeDeAdicoes++;
        UsuarioAdicionado = usuario;

        if (DeveFalharAoAdicionar)
            throw new InvalidOperationException(
                "Falha simulada ao adicionar usuário");

        return Task.CompletedTask;
    }
}



internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string? UltimaSenhaRecebida { get; private set; }

    public string? UltimoHashVerificado { get; private set; }

    public string? UltimaSenhaVerificada { get; private set; }

    public string HashGerado { get; set; } = "hash-gerado";

    public string SenhaCorreta { get; set; } = string.Empty;

    public int QuantidadeDeVerificacoes { get; private set; }

    public string HashPassword(string password)
    {
        UltimaSenhaRecebida = password;
        return HashGerado;
    }

    public bool VerifyPassword(
        string hashedPassword,
        string providedPassword)
    {
        QuantidadeDeVerificacoes++;

        UltimoHashVerificado = hashedPassword;
        UltimaSenhaVerificada = providedPassword;

        return hashedPassword == HashGerado &&
               providedPassword == SenhaCorreta;
    }
}



