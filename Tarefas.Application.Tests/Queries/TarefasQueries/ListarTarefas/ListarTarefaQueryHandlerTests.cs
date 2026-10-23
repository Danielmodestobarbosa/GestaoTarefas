using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;
using Tarefas.Application.Queries.Tarefas.ListarTarefas;

namespace Tarefas.Application.Tests.Queries.Tarefas.ListarTarefas;
public sealed class ListarTarefaQueryHandlerTests
{
    [Fact(DisplayName = "Deve retornar a lista de tarefas")]
    public async Task Deve_Retornar_Lista_De_Tarefas()
    {
        //Arrange
        IReadOnlyList<TarefaResumoDto> tarefasEsperadas =
        [
            new TarefaResumoDto
            {
                TarefaId = Guid.NewGuid(),
                Titulo = "Estudar C#",
                Status = "Pendente",
                Prioridade = "Alta"
            },
            new TarefaResumoDto
            {
                TarefaId = Guid.NewGuid(),
                Titulo = "Estudar testes unitários",
                Status = "EmAndamento",
                Prioridade = "Media"
            }
        ];

        //Act
        var repositorio = new FakeTarefaRepository(tarefasEsperadas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        var resultado = await handler.HandleAsync(query);

        //Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(tarefasEsperadas);
    }

    [Fact(DisplayName = "Deve retornar lista vazia quando não houver tarefas")]
    public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Tarefas()
    {
        //Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        //Act
        var resultado = await handler.HandleAsync(query);

        //Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact(DisplayName = "Deve consultar o repositorio uma única vez")]
    public async Task Deve_Consultar_Repositorio_Uma_Unica_Vez()
    {
        //Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();

        //Act
         await handler.HandleAsync(query);

        //Assert
        repositorio.QuantidadeDeConsultas.Should().Be(1);
    }

    [Fact(DisplayName = "Deve encaminhar o CancellationToken ao repositorio")]
    public async Task Deve_Encaminhar_CancellationToken_Ao_Repositorio()
    {
        //Arrange
        IReadOnlyList<TarefaResumoDto> tarefas = [];

        var repositorio = new FakeTarefaRepository(tarefas);
        var handler = new ListarTarefasQueryHandler(repositorio);
        var query = new ListarTarefasQuery();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        //Act
        await handler.HandleAsync(query, token);

        //Assert
        repositorio.UltimoTokenRecebido.Should().Be(token);
    }
}


internal sealed class FakeTarefaRepository : ITarefaQueryRepository
{
    public int QuantidadeDeConsultas { get; private set; }

    public CancellationToken UltimoTokenRecebido { get; private set; }

    private readonly IReadOnlyList<TarefaResumoDto> _tarefas;

    public FakeTarefaRepository(
            IReadOnlyList<TarefaResumoDto> tarefas)
    {
        _tarefas = tarefas;
    }

    public Task<IReadOnlyList<TarefaResumoDto>> ListarAsync(
            CancellationToken ct = default)
    {
        QuantidadeDeConsultas++;
        UltimoTokenRecebido = ct;

        return Task.FromResult(_tarefas);
    }

    public Task<ObterTarefaPorIdResultDto?>  ObterPorIdAsync (
            Guid tarefaId,
            CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}


