using FluentAssertions;
using Tarefas.Application.Queries.Usuarios.ObterUsuarioExterno;
using Tarefas.Domain.Integration.Usuarios;
using Tarefas.Infrastructure.Fakes;

namespace Tarefas.Application.Tests.Queries.UsuariosQueries.ObterUsuarioExterno;

public sealed class ObterUsuarioExternoQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar usuário quando encontrado")]
    public async Task Deve_Retornar_Usuario_Quando_Encontrado()
    {
        // Arrange
        var usuarioExterno = new UsuarioExternoDto(
            1,
            "Daniel Modesto",
            "daniel@email.com");

        var gateway = new FakeUsuarioGateway();
        var acl = new UsuarioAcl();

        var handler = new ObterUsuarioExternoQueryHandler(gateway, acl);

        var query = new ObterUsuarioExternoQuery(1);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(1);
        resultado.Nome.Should().Be("Daniel Modesto");
        resultado.Email.Should().Be("daniel@email.com");
    }

    [Fact(DisplayName = "Deve retornar null quando usuário não encontrado")]
    public async Task Deve_Retornar_Null_Quando_Usuario_Nao_Encontrado()
    {
        // Arrange
        var gateway = new FakeUsuarioGateway();
        var acl = new UsuarioAcl();

        var handler = new ObterUsuarioExternoQueryHandler(gateway, acl);

        var query = new ObterUsuarioExternoQuery(999);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }
}