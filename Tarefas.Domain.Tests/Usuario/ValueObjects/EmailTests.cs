using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Domain.Tests.Usuário.ValueObjects;

public class EmailTests
{
    #region Teste de Criação

    [Fact(DisplayName = "Deve criar e-mail válido")]
    public void Deve_Criar_Email_Valido()
    {
        //Arrange
        var endereco = "danielmodesto@hotmail.com";

        //Act
        var email = new Email(endereco);

        //Assert
        email.Should().NotBeNull();
        email.Endereco.Should().Be(endereco);
    }

    [Fact(DisplayName = "Deve remover espaços extras do e-mail")]
    public void Deve_Remover_Espacos_Extras_Do_Email()
    {
        //Arrange
        var endereco = "   danielmodesto@hotmail.com   ";

        //Act
        var email = new Email(endereco);

        //Assert
        email.Endereco
            .Should()
            .Be("danielmodesto@hotmail.com");
    }

    #endregion

    #region Testes de validação

    [Fact(DisplayName = "Não deve criar e-mail nulo")]
    public void Nao_Deve_Criar_Email_Nulo()
    {
        //Assert
        string? endereco = null;

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O endereço de email é obrigatório");
    }

    [Fact(DisplayName = "Não deve criar e-mail vazio")]
    public void Nao_Deve_Criar_Email_Vazio()
    {
        //Assert
        var endereco = "";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O endereço de email é obrigatório");
    }

    [Fact(DisplayName = "Não deve criar e-mail com espacos")]
    public void Nao_Deve_Criar_Email_Com_Espacos()
    {
        //Assert
        var endereco = "danielmodesto  @hotmail.com";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O e-mail informado é inválido.");
    }

    [Fact(DisplayName = "Não deve criar e-mail sem @")]
    public void Nao_Deve_Criar_Email_Com_Sem_Arroba()
    {
        //Assert
        var endereco = "danielmodestohotmail.com";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O e-mail informado é inválido.");
    }

    [Fact(DisplayName = "Não deve criar e-mail sem domínio")]
    public void Nao_Deve_Criar_Email_Com_Sem_Dominio()
    {
        //Assert
        var endereco = "danielmodesto@";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O e-mail informado é inválido.");
    }

    [Fact(DisplayName = "Não deve criar e-mail sem ponto do domínio")]
    public void Nao_Deve_Criar_Email_Com_Sem_Ponto_Do_Dominio()
    {
        //Assert
        var endereco = "danielmodesto@hotmailcom";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O e-mail informado é inválido.");
    }

    [Fact(DisplayName = "Não deve criar e-mail com espaços internos")]
    public void Nao_Deve_Criar_Email_Com_Espacos_Internos()
    {
        //Assert
        var endereco = "daniel mode sto@hotmailcom";

        //Act
        var acao = () => new Email(endereco!);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O e-mail informado é inválido.");
    }

    #endregion

    #region Testes de igualdade

    [Fact(DisplayName = "Deve considerar iguais dois e-mails com o mesmo endereco")]
    public void Deve_Considerar_Iguais_Dois_Emails_Com_Mesmo_Endereco()
    {
        //Arrange
        var email11 = "daniel@hotmail.com";
        var email12 = "daniel@hotmail.com";

        //Act e Assert
        email11.Should().Be(email12);
    }

    [Fact(DisplayName = "Deve considerar diferentes dois e-mails com o mesmo endereco")]
    public void Deve_Considerar_Diferentes_Dois_Emails_Com_Mesmo_Endereco()
    {
        //Arrange
        var email11 = "daniel@hotmail.com";
        var email12 = "danielmodesto@hotmail.com";

        //Act e Assert
        email11.Should().NotBe(email12);
    }

    #endregion

    #region Testes de ToString

    [Fact(DisplayName = "Deve retornar o endereço ao chamar ToString()")]
    public void Deve_Retornar_Endereco_Ao_Chamar_ToString()
    {
        // Arrange
        var endereco = "daniel@hotmail.com";
        var email = new Email(endereco);

        // Act
        var resultado = email.ToString();

        // Assert
        resultado.Should().Be(endereco);
    }

    #endregion

}

