using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Tarefas.Enums;
using Tarefas.Domain.Tarefas.Events;

namespace Tarefas.Domain.Tests.Tarefas.Entities;

public class TarefasTests
{
    #region Testes de Criação

    [Fact(DisplayName = "Deve criar tarefa válida com status Pendente")]
    public void Deve_Criar_Tarefa_Valida()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Assert
        tarefa.Should().NotBeNull();
        tarefa.Titulo.Should().Be("Estudar C#");
        tarefa.Descricao.Should().Be("Estudar testes unitários");
        tarefa.Status.Should().Be(StatusTarefa.Pendente);
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        tarefa.Id.Should().NotBeEmpty();
        tarefa.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TarefaCriadaEvent>();

    }

    [Fact(DisplayName = "Não deve criar tarefa com titulo menor que 3 caracteres")]
    public void Nao_Deve_Criar_Tarefa_Com_Titulo_Menor_Que_Tres_Caracteres()
    {
        //Arrange
        var titulo = "AB";
        var descricao = "Estudar testes unitários";
        var prioridade = PrioridadeTarefa.Alta;

        //Act 
        var acao = () => new Tarefa(titulo, descricao, prioridade);

        //Assert
        acao.Should().
            Throw<DomainException>().
            WithMessage("O título deve possuir entre 3 e 100 caracteres.");
    }

    [Fact(DisplayName = "Não deve criar tarefa com titulo maior que 100 caracteres")]
    public void Nao_Deve_Criar_Tarefa_Com_Titulo_Maior_Que_Cem_Caracteres()
    {
        //Arrange
        var titulo = new string('A', 101);
        var descricao = "Estudar testes unitarios";
        var prioridade = PrioridadeTarefa.Alta;

        //Act 
        var acao = () => new Tarefa(titulo, descricao, prioridade);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O título deve possuir entre 3 e 100 caracteres.");
    }

    [Theory(DisplayName = "Não deve criar tarefa com titulo inválido")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Nao_Deve_Criar_Tarefa_Com_Titulo_Invalido(string? titulo)
    {
        //Arrange
        var descricao = "Estudar testes unitários";
        var prioridade = PrioridadeTarefa.Alta;

        //Act
        var acao = () => new Tarefa(titulo!, descricao, prioridade);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O título da tarefa é obrigatório.");
    }

    [Fact(DisplayName = "Não deve criar tarefa com prioridade inválida")]
    public void Nao_Deve_Criar_Tarefa_Com_Prioridade_Invalida()
    {
        //Arrange
        var titulo = "Estudar C#";
        var descricao = "Estudar testes unitários";
        var prioridade = (PrioridadeTarefa)999;

        //Act
        var acao = () => new Tarefa(titulo, descricao, prioridade);

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A prioridade da tarefa é inválida.");
    }

    [Fact(DisplayName = "Deve criar tarefa com a descrição nula")]
    public void Criar_Tarefa_Com_Descricao_Nula()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            null,
            PrioridadeTarefa.Alta);

        //Assert
        tarefa.Should().NotBeNull();
        tarefa.Titulo.Should().Be("Estudar C#");
        tarefa.Descricao.Should().BeNull();
        tarefa.Status.Should().Be(StatusTarefa.Pendente);
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Alta);
        tarefa.Id.Should().NotBeEmpty();
        tarefa.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TarefaCriadaEvent>();

    }

    [Fact(DisplayName = "Deve remover espaços extras do título e da descrição")]
    public void Deve_Remover_Espacos_Extras_Do_Titulo_E_Descricao()
    {
        // Arrange
        var tarefa = new Tarefa(
            "  Estudar C#  ",
            "  Estudar testes unitários  ",
            PrioridadeTarefa.Alta);

        // Act
        // O Trim() é executado automaticamente no construtor.

        // Assert
        tarefa.Titulo.Should().Be("Estudar C#");
        tarefa.Descricao.Should().Be("Estudar testes unitários");
    }

    #endregion

    #region Testes de Iniciar Tarefa

    [Fact(DisplayName = "Deve iniciar uma tarefa pendente")]
    public void Deve_Iniciar_Tarefa_Pendente()
    {
        //Arrange 
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Iniciar();

        //Assert
        tarefa.Status.Should().Be(StatusTarefa.EmAndamento);
    }

    [Fact(DisplayName = "Deve registrar evento ao iniciar tarefa")]
    public void Deve_Registrar_Evento_Ao_Iniciar_Tarefa()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Iniciar();

        //Assert
        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaIniciadaEvent);
    }

    [Fact(DisplayName = "Deve atualizar a DataAtualização")]
    public void Deve_Atualizar_A_DataAtualizacao()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Iniciar();

        //Assert
        tarefa.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve iniciar tarefa que não está pendente")]
    public void Nao_Deve_Iniciar_Tarefa_Se_Nao_Estiver_Pendente()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        tarefa.Iniciar();

        //Act
        var acao = () => tarefa.Iniciar();

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A tarefa não pode ser iniciada pois não está pendente.");
        
    }

    [Fact(DisplayName = "Não deve duplicar evento ao iniciar tarefa novamente")]
    public void Nao_Deve_Duplicar_Evento_Ao_Iniciar_Tarefa_Novamente()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        // Act
        tarefa.Iniciar();

        var acao = () => tarefa.Iniciar();

        // Assert
        acao.Should().Throw<DomainException>();

        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaIniciadaEvent);
    }

    #endregion

    #region Testes de Concluir Tarefa

    [Fact(DisplayName = "Deve alterar o status para Concluída")]
    public void Deve_Alterar_Status_Para_Concluida()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act 
        tarefa.Concluir();

        //Assert
        tarefa.Status.Should().Be(StatusTarefa.Concluida);
    }

    [Fact(DisplayName = "Deve definir DataConclusao")]
    public void Deve_Definir_DataConclusao()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act 
        tarefa.Concluir(); 

        //Assert
        tarefa.DataConclusao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve registrar TarefaConcluidaEvent")]
    public void Deve_Registrar_TarefaConcluidaEvent()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Concluir();

        //Assert
        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaConcluidaEvent);
    }

    [Fact(DisplayName = "Não deve duplicar evento ao concluir novamente")]
    public void Nao_Duplica_Evento_Ao_Concluir_Novamente()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Concluir();
        tarefa.Concluir();

        //Assert
        tarefa.DomainEvents.Should().ContainSingle(e => e is TarefaConcluidaEvent);

    }

    [Fact(DisplayName = "Não deve concluir tarefa cancelada")]
    public void Nao_Deve_Concluir_Tarefa_Cancelada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        tarefa.Cancelar();

        //Act
        var acao = () => tarefa.Concluir();

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A tarefa não pode ser concluída pois está cancelada.");

    }

    [Fact(DisplayName = "Não deve duplicar evento ao concluir novamente e manter data de conclusão")]
    public void Nao_Deve_Duplicar_Evento_Ao_Concluir_Novamente_E_Manter_DataConclusao()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Concluir();
        var dataConclusaoOriginal = tarefa.DataConclusao;

        tarefa.Concluir();

        // Assert
        tarefa.DataConclusao.Should().Be(dataConclusaoOriginal);

        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaConcluidaEvent);
    }

    #endregion

    #region Testes de Atualizar tarefa

    [Fact(DisplayName = "Deve atualizar titulo, descrição e prioridade")]
    public void Deve_Atualizar_Titulo_Descricao_Prioridade()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Atualizar(
            "Estudar C# WebApi",
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Baixa);

        //Assert
        tarefa.Titulo.Should().Be("Estudar C# WebApi");
        tarefa.Descricao.Should().Be("Estudar testes unitários de tarefa");
        tarefa.Prioridade.Should().Be(PrioridadeTarefa.Baixa);
    }

    [Fact(DisplayName = "Não deve atualizar tarefa cancelada")]
    public void Nao_Deve_Atualizar_Tarefa_Cancelada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        tarefa.Cancelar();

        //Act
        var acao = () => tarefa.Atualizar(
            "Estudar C# WebApi",
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Baixa);

        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A tarefa não pode ser atualizada pois está cancelada.");
    }

    [Fact(DisplayName = "Deve registrar TarefaAtualizadaEvent")]
    public void Deve_Registrar_TarefaAtualizadaEvent()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Atualizar(
            "Estudar C# WebApi",
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Baixa);

        //Assert
        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaAtualizadaEvent);
    }

    [Fact(DisplayName = "Não deve atualizar tarefa com titulo com menos de 3 caracteres")]
    public void Nao_Deve_Atualizar_Tarefa_Com_Titulo_Com_Menos_De_Tres_Caracteres()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        var acao = () => tarefa.Atualizar(
            "AB",
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Alta);

        acao.Should().Throw<DomainException>().WithMessage("O título deve possuir entre 3 e 100 caracteres.");
    }

    [Fact(DisplayName = "Não deve atualizar tarefa com titulo com mais de 100 caracteres")]
    public void Nao_Deve_Atualizar_Tarefa_Com_Titulo_Com_Mais_De_Cem_Caracteres()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        var acao = () => tarefa.Atualizar(
            new string ('A', 101),
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Alta);

        acao.Should().Throw<DomainException>().WithMessage("O título deve possuir entre 3 e 100 caracteres.");
    }

    [Fact(DisplayName = "Não deve atualizar tarefa com prioridade inválida")]
    public void Nao_Deve_Atualizar_Tarefa_Com_Prioridade_Invalida()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        var acao = () => tarefa.Atualizar(
            "Estudar C#",
            "Estudar testes unitários de tarefa",
            (PrioridadeTarefa)999);

        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A prioridade da tarefa é inválida.");
    }

    [Fact(DisplayName = "Deve atualizar DataAtualização ao atualizar tarefa")]
    public void Deve_Atualizar_DataAtualizacao_Ao_Atualizar_Tarefa()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Atualizar(
            "Estudar C# WebApi",
            "Estudar testes unitários de tarefa",
            PrioridadeTarefa.Baixa);

        //Assert
        tarefa.DataAtualizacao.Should().NotBeNull();
    }

    #endregion

    #region Testes de Cancelar tarefa

    [Fact(DisplayName = "Deve alterar o status para Cancelada")]
    public void Deve_Alterar_Status_Para_Cancelada()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Cancelar();

        //Assert
        tarefa.Status.Should().Be(StatusTarefa.Cancelada);
    }

    [Fact(DisplayName = "Deve registrar TarefaCanceladaEvent")]
    public void Deve_Registrar_TarefaCanceladaEvent()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Cancelar();

        //Assert
        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaCanceladaEvent);
    }

    [Fact(DisplayName = "Não duplicar evento ao cancelar novamente")]
    public void Nao_Deve_Duplicata_Event_Ao_Cancelar_Tarefa_Novamente()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Cancelar();
        tarefa.Cancelar();

        //Assert
        tarefa.DomainEvents
            .Should()
            .ContainSingle(e => e is TarefaCanceladaEvent);

    }

    [Fact(DisplayName = "Não deve cancelar tarefa concluída")]
    public void Nao_Deve_Cancelar_Tarefa_Concluida()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        tarefa.Concluir();

        //Act
        var acao = () => tarefa.Cancelar();

        //Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage("A tarefa não pode ser cancelada pois está concluída.");
    }

    [Fact(DisplayName = "Deve atualizar DataAtualização ao cancelar a tarefa")]
    public void Deve_Atualizar_DataAtualizacao_Ao_Cancelar_Tarefa()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        tarefa.Cancelar();

        //Assert
        tarefa.DataAtualizacao.Should().NotBeNull();
    }

    #endregion

    #region Teste de AtribuirUsuario

    [Fact(DisplayName = "Deve atribuir um usuario válido a tarefa")]
    public void Deve_Atribuir_Usuario_Valido_A_Tarefa()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuarioId = Guid.NewGuid();

        //Act
        tarefa.AtribuirUsuario(usuarioId);

        //Assert
        tarefa.UsuarioId.Should().Be(usuarioId);

    }

    [Fact(DisplayName = "Não deve permitir atribuir com ID inválido")]
    public void Nao_Deve_Atribuir_Usuario_Com_Id_Invalido()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        //Act
        var acao = () => tarefa.AtribuirUsuario(Guid.Empty);

        //Assert
        acao.Should().
            Throw<DomainException>()
            .WithMessage("O ID do usuário é inválido");
    }

    [Fact(DisplayName = "Deve atualizar a data ao atribuir usuario")]
    public void Deve_Atribuir_Data_Ao_Atribuir_Usuario()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var usuarioId = Guid.NewGuid();

        //Act
        tarefa.AtribuirUsuario(usuarioId);

        //Assert
        tarefa.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve permitir reatribuir uma tarefa a outro usuário")]
    public void Deve_Permitir_Reatribuir_Tarefa_Para_Outro_Usuario()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var primeiroUsuáioId = Guid.NewGuid();
        var segundoUsuarioId = Guid.NewGuid();
 
        tarefa.AtribuirUsuario(primeiroUsuáioId);

        //Act
        tarefa.AtribuirUsuario(segundoUsuarioId);

        //Assert
        tarefa.UsuarioId.Should().Be(segundoUsuarioId);
    }

    [Fact(DisplayName = "Não deve permitir substituir o usuário quando o novo ID é inválido")]
    public void Nao_Deve_Permitir_Atribuir_Outro_Usuario_Se_Id_For_Inválido()
    {
        //Arrange
        var tarefa = new Tarefa(
            "Estudar C#",
            "Estudar testes unitários",
            PrioridadeTarefa.Alta);

        var primeiroUsuáioId = Guid.NewGuid();

        tarefa.AtribuirUsuario(primeiroUsuáioId);

        //Act
        var acao = () => tarefa.AtribuirUsuario(Guid.Empty);

        //Assert
        acao.Should().
            Throw<DomainException>();

        tarefa.UsuarioId.Should().Be(primeiroUsuáioId);
    }

    #endregion
}

