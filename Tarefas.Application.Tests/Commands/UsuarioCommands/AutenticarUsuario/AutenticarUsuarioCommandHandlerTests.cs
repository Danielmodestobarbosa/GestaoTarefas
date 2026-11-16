using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;
using Tarefas.Application.Tests.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioCommandHandlerTests
{

    [Fact(DisplayName = "Deve autenticar usuário quando as credenciais forem válidas")]
    public async Task HandleAsync_DeveAutenticarUsuario_QuandoCredenciaisForemValidas()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");
        const string senha = "SenhaSegura123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);

        usuario.DefinirSenha(passwordHash);

        var repositorio = new FakeUsuarioRepository(usuario);
        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = passwordHash
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        // Act
        var resultado = await handler.Handle(
            command, 
            CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Should().Be(nome);
        resultado.Email.Should().Be(email);
    }


    [Fact(DisplayName = "Deve rejeitar autenticação quando usuário não existir")]
    public async Task HandleAsync_DeveRejeitarAutenticacao_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var email = new Email("inexistente@email.com");
        const string senha = "SenhaSegura123";

        var repositorio = new FakeUsuarioRepository();

        var passwordHasher = new FakePasswordHasher();

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        // Act
        Func<Task> act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("E-mail ou senha inválidos.");
    }


    [Fact(DisplayName = "Deve rejeitar autenticação quando usuário estiver inativo")]
    public async Task HandleAsync_DeveRejeitarAutenticacao_QuandoUsuarioEstiverInativo()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");

        const string senha = "SenhaSegura123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);
        usuario.DefinirSenha(passwordHash);
        usuario.DesativarPerfil();

        var repositorio = new FakeUsuarioRepository(usuario);

        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = passwordHash
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        // Act
        Func<Task> act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("E-mail ou senha inválidos.");
    }


    [Fact(DisplayName = "Deve rejeitar autenticação quando senha estiver incorreta")]
    public async Task HandleAsync_DeveRejeitarAutenticacao_QuandoSenhaEstiverIncorreta()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");

        const string senhaIncorreta = "SenhaErrada123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);
        usuario.DefinirSenha(passwordHash);

        var repositorio = new FakeUsuarioRepository(usuario);

        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = "outro-hash"
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(
            email,
            senhaIncorreta);

        // Act
        Func<Task> act = async () => await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("E-mail ou senha inválidos.");
    }


    [Fact(DisplayName = "Deve consultar usuário utilizando o e-mail informado")]
    public async Task HandleAsync_DeveConsultarUsuario_UtilizandoEmailInformado()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");

        const string senha = "SenhaSegura123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);
        usuario.DefinirSenha(passwordHash);

        var repositorio = new FakeUsuarioRepository(usuario);

        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = passwordHash
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositorio.UltimoEmailConsultado.Should()
            .Be(email.Endereco);
    }


    [Fact(DisplayName = "Deve repassar CancellationToken ao consultar usuário")]
    public async Task HandleAsync_DeveRepassarCancellationToken_AoConsultarUsuario()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");

        const string senha = "SenhaSegura123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);
        usuario.DefinirSenha(passwordHash);

        var repositorio = new FakeUsuarioRepository(usuario);

        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = passwordHash
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenConsulta.Should()
            .Be(cancellationToken);
    }

    [Fact(DisplayName = "Deve verificar senha utilizando hash e senha informados")]
    public async Task HandleAsync_DeveVerificarSenha_UtilizandoHashESenhaInformados()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel");
        var email = new Email("daniel@email.com");

        const string senha = "SenhaSegura123";
        const string passwordHash = "hash-gerado";

        var usuario = new Usuario(nome, email);
        usuario.DefinirSenha(passwordHash);

        var repositorio = new FakeUsuarioRepository(usuario);

        var passwordHasher = new FakePasswordHasher
        {
            HashGerado = passwordHash,
            SenhaCorreta = senha
        };

        var handler = new AutenticarUsuarioCommandHandler(
            repositorio,
            passwordHasher);

        var command = new AutenticarUsuarioCommand(email, senha);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        passwordHasher.UltimoHashVerificado.Should().Be(passwordHash);
        passwordHasher.UltimaSenhaVerificada.Should().Be(senha);
        passwordHasher.QuantidadeDeVerificacoes.Should().Be(1);
    }
}

