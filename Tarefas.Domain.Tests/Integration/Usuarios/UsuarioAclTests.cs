using FluentAssertions;
using Tarefas.Domain.Integration.Usuarios;

namespace Tarefas.Domain.Tests.Integration.Usuarios;

public sealed class UsuarioAclTests
{
    [Fact(DisplayName = "Deve traduzir os dados do usuário externo corretamente")]
    public void Deve_Traduzir_Dados_Usuario_Externo_Corretamente()
    {
        // Arrange
        var dto = new UsuarioExternoDto(
            id: 1,
            nome: "Daniel Modesto",
            email: "daniel@email.com");

        var acl = new UsuarioAcl();

        // Act
        var resultado = acl.TraduzirUsuario(dto);

        // Assert
        resultado.Nome.Should().Be("Daniel Modesto");
        resultado.Email.Should().Be("daniel@email.com");
    }
}