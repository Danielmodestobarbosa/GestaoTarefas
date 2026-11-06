using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;
using Tarefas.Application.Exceptions;
using Tarefas.Application.Tests.Commands.FakesUnitOfWork;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommandHandlerTests
{
    #region Testes de desativação do usuário

    [Fact(DisplayName = "Deve desativar o usuário com sucesso")]
    public async Task HandleAsync_DeveDesativarUsuario_ComSucesso()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio, 
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Should().Be(nome);
        resultado.Email.Should().Be(email);
        resultado.Status.Should().Be(StatusUsuario.Inativo);

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    [Fact(DisplayName = "Deve lançar exceção quando o usuário não for encontrado")]
    public async Task HandleAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var command = new DesativarUsuarioCommand(usuarioId);

        var repositorio = new FakeUsuarioRepository(null);

        var unitOfWork = new FakeUnitOfWork();
        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Usuario não encontrado");

        repositorio.UltimoIdConsultado.Should().Be(usuarioId);
        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve propagar exceção quando falhar ao consultar usuário")]
    public async Task HandleAsync_DevePropagarExcecao_QuandoFalharAoConsultar()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario)
        {
            DeveFalharAoConsultar = true
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao consultar usuário");

        unitOfWork.QuantidadeDeCommits.Should().Be(0);
    }


    [Fact(DisplayName = "Deve repassar o CancellationToken ao repositório")]
    public async Task HandleAsync_DeveRepassarCancellationToken()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        repositorio.UltimoTokenConsulta.Should().Be(cancellationToken);
        unitOfWork.UltimoTokenCommit.Should().Be(cancellationToken);
    }


    [Fact(DisplayName = "Deve consultar o usuário pelo ID informado")]
    public async Task HandleAsync_DeveConsultarUsuarioPeloIdInformado()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        await handler.Handle(command);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(usuario.Id);
    }


    [Fact(DisplayName = "Deve propagar exceção quando falhar ao atualizar usuário")]
    public async Task HandleAsync_DevePropagarExcecao_QuandoFalharAoAtualizar()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork
        {
            ExcecaoAoCommit = new InvalidOperationException(
                "Falha simulada ao atualizar usuário")
        };

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar usuário");

        unitOfWork.QuantidadeDeCommits.Should().Be(1);
        usuario.Status.Should().Be(StatusUsuario.Inativo);
    }


    [Fact(DisplayName = "Deve preservar os dados do usuário ao desativá-lo")]
    public async Task HandleAsync_DevePreservarDadosDoUsuario()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Should().Be(nome);
        resultado.Email.Should().Be(email);

        usuario.Nome.Should().Be(nome);
        usuario.Email.Should().Be(email);
    }


    [Fact(DisplayName = "Deve retornar status Inativo após desativar o usuário")]
    public async Task HandleAsync_DeveRetornarStatusInativo()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        var resultado = await handler.Handle(command);

        // Assert
        resultado.Status.Should().Be(StatusUsuario.Inativo);
        usuario.Status.Should().Be(StatusUsuario.Inativo);
    }


    [Fact(DisplayName = "Deve atualizar o usuário apenas uma vez")]
    public async Task HandleAsync_DeveAtualizarUsuarioApenasUmaVez()
    {
        // Arrange
        var usuario = new Usuario(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"));

        var command = new DesativarUsuarioCommand(usuario.Id);

        var repositorio = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new DesativarUsuarioCommandHandler(
            repositorio,
            unitOfWork);

        // Act
        await handler.Handle(command);

        // Assert
        unitOfWork.QuantidadeDeCommits.Should().Be(1);
    }


    #endregion
}
internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario _usuario;

    public bool DeveFalharAoConsultar { get; set; }

    public CancellationToken UltimoTokenConsulta { get; private set; }

    public Guid UltimoIdConsultado { get; private set; }
    public Usuario? UsuarioAtualizado { get; private set; }

    public FakeUsuarioRepository(Usuario? usuario = null)
    {
        _usuario = usuario;
    }

    public Task<Usuario?> ObterUsuarioPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        UltimoIdConsultado = usuarioId;
        UltimoTokenConsulta = cancellationToken;

        if (DeveFalharAoConsultar)
            throw new InvalidOperationException(
                "Falha simulada ao consultar usuário");

        return Task.FromResult<Usuario?>(
            _usuario?.Id == usuarioId ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}