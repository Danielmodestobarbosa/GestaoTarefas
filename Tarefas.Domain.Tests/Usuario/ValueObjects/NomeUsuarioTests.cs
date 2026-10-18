using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Domain.Tests.Usuário.ValueObjects;

public class NomeUsuarioTests
{
    #region Testes de Criação

    [Fact(DisplayName = "Deve criar nome de usuário válido")]
    public void Deve_Criar_Nome_De_Usuario_Valido()
    {
        //Arrange e Act
        var nome = new NomeUsuario("Daniel Modesto");

        //Assert
        nome.Should().NotBeNull();
        nome.Nome.Should().Be("Daniel Modesto");
    }

    [Fact(DisplayName = "Deve remover espaços extras do nome")]
    public void Deve_Remover_Espacos_Extras_Do_Nome()
    {
        //Arrange e Act
        var nome = new NomeUsuario("   Daniel Modesto   ");

        //Assert
        nome.Nome.Should().Be("Daniel Modesto");
    }

    #endregion

    #region Testes de Validação

    [Fact(DisplayName = "Não deve criar nome nulo")]
    public void Nao_Deve_Criar_Nome_Nulo()
    {
        //Arrange
        string? nome = null;

        //Act
        var acao = () => new NomeUsuario(nome);

        //Assert
        acao.Should().Throw<DomainException>().WithMessage("O nome do usuário não pode ser nulo ou vazio.");
    }

    [Fact(DisplayName = "Não deve criar nome vazio")]
    public void Nao_Deve_Criar_Nome_vazio()
    {
        //Arrange
        var nome = "";

        //Act
        var acao = () => new NomeUsuario(nome);

        //Assert
        acao.Should().Throw<DomainException>().WithMessage("O nome do usuário não pode ser nulo ou vazio.");
    }

    [Fact(DisplayName = "Não deve criar nome com espaços")]
    public void Nao_Deve_Criar_Nome_Com_Espacos()
    {
        //Arrange
        var nome = "     ";

        //Act
        var acao = () => new NomeUsuario(nome);

        //Assert
        acao.Should().Throw<DomainException>().WithMessage("O nome do usuário não pode ser nulo ou vazio.");
    }

    [Fact(DisplayName = "Não deve criar nome com menos de 3 caracteres")]
    public void Nao_Deve_Criar_Nome_Com_Menos_De_Tres_Caracteres()
    {
        //Arrange
        var nome = "AB";

        //Act
        var acao = () => new NomeUsuario(nome);

        //Assert
        acao.Should().Throw<DomainException>().WithMessage("O nome deve possuir entre 3 e 100 caracteres");
    }

    [Fact(DisplayName = "Não deve criar nome com mais de 100 caracteres")]
    public void Nao_Deve_Criar_Nome_Com_Mais_De_Cem_Caracteres()
    {
        //Arrange
        string nome = new string('A', 101);

        //Act
        var acao = () => new NomeUsuario(nome);

        //Assert
        acao.Should().Throw<DomainException>().WithMessage("O nome deve possuir entre 3 e 100 caracteres");
    }

    [Fact(DisplayName = "Deve aceitar nome com exatamente 3 caracteres")]
    public void Deve_Aceitar_Nome_Com_Tres_Caracteres()
    {
        //Arrange e Act
        var nome = new NomeUsuario("ABC");

        //Assert
        nome.Nome.Should().Be("ABC");
    }

    [Fact(DisplayName = "Deve aceitar nome com exatamente 100 caracteres")]
    public void Deve_Aceitar_Nome_Com_Cem_Caracteres()
    {
        //Arrange e Act
        string nome = new string('A', 100);
        var nomeUsuario = new NomeUsuario(nome);

        //Assert
        nomeUsuario.Nome.Should().Be(nome);
        nomeUsuario.Nome.Length.Should().Be(100);
    }

    #endregion

    #region Testes de Igualdade

    [Fact(DisplayName = "Deve considerar iguais dois nomes com o mesmo valor")]
    public void Deve_Considerar_Iguais_Dois_Nomes_Com_Mesmo_Valor()
    {
        //Arrange
        var nome11 = "Daniel Modesto";
        var nome12 = "Daniel Modesto";

        //Assert
        nome11.Should().Be(nome12);
    }

    [Fact(DisplayName = "Deve considerar diferentes nomes com valores diferentes")]
    public void Deve_Considerar_Diferentes_Nome_Com_Valores_Diferentes()
    {
        //Arrange
        var nome11 = "Daniel Modesto";
        var nome12 = "Daniel Modesto Barbosa";

        //Assert
        nome11.Should().NotBe(nome12);
    }

    #endregion

    #region Testes de ToString()

    [Fact(DisplayName = "Deve retornar o nome ao chamar ToString()")]
    public void Deve_Retornar_Nome_Ao_Chamar_ToString()
    {
        // Arrange
        var nome = "Daniel Modesto";
        var nomeUsuario = new NomeUsuario(nome);

        // Act
        var resultado = nomeUsuario.ToString();

        // Assert
        resultado.Should().Be(nome);
    }

    #endregion
}

