
using FluentAssertions;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Infrastructure.Security;

namespace Tarefas.Infrastructure.Tests.Security;

public class PasswordHasherServiceTests
{
    private readonly PasswordHasherService _passwordHasher;

    public PasswordHasherServiceTests()
    {
        _passwordHasher = new PasswordHasherService();
    }

    [Fact(DisplayName = "Deve gerar hash diferente da senha original")]
    public void DeveGerarHashDiferenteDaSenhaOriginal()
    {
        //Arrange
        const string password = "Senha@123";

        //Act
        var hash = _passwordHasher.HashPassword(password);

        //Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact(DisplayName = "Deve gerar hashes diferentes para a mesma senha")]
    public void DeveGerarHashesDiferentesParaMesmaSenha()
    {
        //Arrange
        const string password = "Senha@123";

        //Act
        var firstHash = _passwordHasher.HashPassword(password);
        var secondHash = _passwordHasher.HashPassword(password);

        //Assert
        firstHash.Should().NotBe(secondHash);
    }

    [Fact(DisplayName = "Deve validar senha correta")]
    public void Deve_Validar_Senha_Correta()
    {
        // Arrange
        const string password = "Senha@123";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(hash, password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve rejeitar senha incorreta ")]
    public void Deve_Rejeitar_Senha_Incorreta()
    {
        // Arrange
        const string password = "Senha@123";
        const string incorrectPassword = "Senha@456";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(hash, incorrectPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Theory(DisplayName = "Deve rejeitar hash inválido")]
    [InlineData("")]
    [InlineData("hash_invalido")]
    public void Deve_Rejeitar_Hash_Invalido(string invalidHash)
    {
        // Arrange
        const string password = "Senha@123";

        // Act
        var result = _passwordHasher.VerifyPassword(invalidHash, password);

        // Assert
        result.Should().BeFalse();
    }

    [Theory(DisplayName = "Deve rejeitar senhas vazias ou em branco")]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Rejeitar_Senha_Vazia_Ou_Em_Branco(
    string invalidPassword)
    {
        // Arrange

        // Act
        var action = () => _passwordHasher.HashPassword(invalidPassword);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}




