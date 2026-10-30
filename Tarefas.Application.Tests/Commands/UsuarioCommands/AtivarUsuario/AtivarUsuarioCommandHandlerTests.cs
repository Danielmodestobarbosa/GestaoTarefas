using FluentAssertions;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioCommandHandlerTests
{
    #region Testes de Ativação 

    [Fact(DisplayName = "Deve ativar usuário quando encontrado")]
    public async Task Deve_Ativar_Usuario_Quando_Encontrado()
    {
        //Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        usuario.DesativarPerfil();

        var repositorio = new FakeUsuarioRepository(usuario);
        var handler = new AtivarUsuarioCommandHandler(repositorio);

        var command = new AtivarUsuarioCommand(usuario.Id);

        //Act 
        var resultado = await handler.Handle(command);

        //Assert
        resultado.UsuarioId.Should().Be(usuario.Id);
        resultado.Nome.Nome.Should().Be("Daniel Modesto");
        resultado.Email.Endereco.Should().Be("daniel@hotmail.com");
        resultado.Status.Should().Be(StatusUsuario.Ativo);

        repositorio.QuantidadeDeAtualizacoes.Should().Be(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o usuário não for encontrado")]
    public async Task Deve_Lancar_Excecao_Quando_Usuario_Nao_Encontrado()
    {
        //Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var repositorio = new FakeUsuarioRepository(usuario);
        var handler = new AtivarUsuarioCommandHandler(repositorio);

        var command = new AtivarUsuarioCommand(Guid.NewGuid());

        //Act 
        var acao = async () => await handler.Handle(command);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Usuario não encontrado");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(0);
    }

    [Fact(DisplayName = "Deve propagar exceção quando falhar ao consultar")]
    public async Task Deve_Propagar_Excecao_Ao_Falhar_Ao_Consultar()
    {
        //Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var repositorio = new FakeUsuarioRepository(usuario)
        {
            DeveFalharAoConsultar = true
        };

        var handler = new AtivarUsuarioCommandHandler(repositorio);

        var command = new AtivarUsuarioCommand(Guid.NewGuid());

        //Act 
        var acao = async () => await handler.Handle(command);

        //Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao consultar usuário");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(0);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositório")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        //Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var repositorio = new FakeUsuarioRepository(usuario);
        var handler = new AtivarUsuarioCommandHandler(repositorio);

        var command = new AtivarUsuarioCommand(usuario.Id);

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

    [Fact(DisplayName = "Deve consultar usando o ID correto do usuario")]
    public async Task Deve_Consultar_Usando_Id_Correto_Do_Usuario()
    {
        //Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");

        var usuario = new Usuario(nome, email);

        var repositorio = new FakeUsuarioRepository(usuario);
        var handler = new AtivarUsuarioCommandHandler(repositorio);

        var command = new AtivarUsuarioCommand(usuario.Id);

        // Act
        await handler.Handle(command);

        // Assert
        repositorio.UltimoIdConsultado.Should().Be(usuario.Id);
    }


    [Fact(DisplayName = "Deve propagar exceção quando falhar ao atualizar")]

    public async Task Deve_Propagar_Excecao_Ao_Falhar_Ao_Atualizar()
    {
        // Arrange
        var nome = new NomeUsuario("Daniel Modesto");
        var email = new Email("daniel@hotmail.com");
        var usuario = new Usuario(nome, email);

        usuario.DesativarPerfil();

        var repositorio = new FakeUsuarioRepository(usuario)
        {
            DeveFalharAoAtualizar = true
        };

        var handler = new AtivarUsuarioCommandHandler(repositorio);
        var command = new AtivarUsuarioCommand(usuario.Id);

        // Act
        var acao = async () => await handler.Handle(command);

        // Assert
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha simulada ao atualizar usuário");

        repositorio.QuantidadeDeAtualizacoes.Should().Be(1);
    }


    #endregion
}

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario _usuario;
    public int QuantidadeDeAtualizacoes { get; private set; }
    public bool DeveFalharAoConsultar { get; set; }
    public bool DeveFalharAoAtualizar { get; set; }
    public CancellationToken UltimoTokenConsulta { get; private set; }
    public CancellationToken UltimoTokenAtualizacao { get; private set; }
    public Guid UltimoIdConsultado { get; private set; }
    public FakeUsuarioRepository(Usuario usuario)
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
            usuarioId == _usuario.Id ? _usuario : null);
    }

    public Task AdicionarAsync(
        Usuario usuario, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AtualizarAsync(
        Usuario usuario, 
        CancellationToken cancellationToken = default)
    {
        UltimoTokenAtualizacao = cancellationToken;

        QuantidadeDeAtualizacoes++;

        if (DeveFalharAoAtualizar)
            throw new InvalidOperationException(
                "Falha simulada ao atualizar usuário");

        return Task.CompletedTask;
    }

}
