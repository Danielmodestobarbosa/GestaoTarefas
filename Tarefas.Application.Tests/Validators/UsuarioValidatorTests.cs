
using FluentAssertions;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Validators.Usuario;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Tests.Validators.Usuario;

public sealed class CriarUsuarioCommandValidatorTests
{
    private readonly CriarUsuarioCommandValidator _validator = new();

    #region Testes de Validação da Senha

    [Fact(DisplayName = "Deve validar comando com senha válida")]
    public void Deve_Validar_Comando_Com_Senha_Valida()
    {
        // Arrange
        var command = CriarCommand("Senha@123");

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Theory(DisplayName = "Não deve aceitar senha vazia ou nula")]
    [InlineData(null)]
    [InlineData("")]
    public void Nao_Deve_Aceitar_Senha_Vazia_Ou_Nula(string? senha)
    {
        // Arrange
        var command = CriarCommand(senha);

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();

        resultado.Errors.Should().Contain(erro =>
            erro.PropertyName == nameof(CriarUsuarioCommand.Senha) &&
            erro.ErrorMessage == "A senha do usuário é obrigatória.");
    }

    [Fact(DisplayName = "Não deve aceitar senha contendo apenas espaços")]
    public void Nao_Deve_Aceitar_Senha_Com_Apenas_Espacos()
    {
        // Arrange
        var command = CriarCommand("        ");

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();

        resultado.Errors.Should().Contain(erro =>
            erro.PropertyName == nameof(CriarUsuarioCommand.Senha) &&
            erro.ErrorMessage == "A senha não pode conter apenas espaços.");
    }

    [Fact(DisplayName = "Não deve aceitar senha com menos de 8 caracteres")]
    public void Nao_Deve_Aceitar_Senha_Com_Menos_De_Oito_Caracteres()
    {
        // Arrange
        var command = CriarCommand("Abc123!");

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();

        resultado.Errors.Should().Contain(erro =>
            erro.PropertyName == nameof(CriarUsuarioCommand.Senha) &&
            erro.ErrorMessage == "A senha deve possuir no mínimo 8 caracteres.");
    }

    [Fact(DisplayName = "Não deve aceitar senha com mais de 128 caracteres")]
    public void Nao_Deve_Aceitar_Senha_Com_Mais_De_Cento_E_Vinte_Oito_Caracteres()
    {
        // Arrange
        var senha = new string('A', 129);
        var command = CriarCommand(senha);

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.IsValid.Should().BeFalse();

        resultado.Errors.Should().Contain(erro =>
            erro.PropertyName == nameof(CriarUsuarioCommand.Senha) &&
            erro.ErrorMessage == "A senha deve possuir no máximo 128 caracteres.");
    }

    #endregion

    private static CriarUsuarioCommand CriarCommand(string? senha)
    {
        return new CriarUsuarioCommand(
            new NomeUsuario("Daniel Modesto"),
            new Email("daniel@hotmail.com"),
            senha!);
    }
}
