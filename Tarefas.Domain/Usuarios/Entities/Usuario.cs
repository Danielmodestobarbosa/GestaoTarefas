using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Base;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Common.Validations;
using Tarefas.Domain.Tarefas.Enums;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.Events;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Domain.Usuarios.Entities;

public sealed class Usuario : AggregateRoot
{
    public NomeUsuario Nome { get; private set; }
    public Email Email { get; private set; }
    public StatusUsuario Status { get; private set; }

    // Construtor utilizado pelo Entity Framework Core.
    private Usuario()
    {
        Nome = null!;
        Email = null!;
    }

    public Usuario(
        NomeUsuario nome,
        Email email)
    {
        Validar(nome, email);

        Nome = nome;
        Email = email;
        Status = StatusUsuario.Ativo;

        AddDomainEvent(new UsuarioCriadoEvent(
            UsuarioId: Id,
            Nome: Nome.Nome,
            Email: Email.Endereco));
    }

    public void AtualizarPerfil(
        NomeUsuario nome,
        Email email)
    {
        Guard.Against<DomainException>(
            Status == StatusUsuario.Inativo,
            "Usuários inativos não podem ter o perfil atualizado");

        Validar(nome, email);

        Nome = nome;
        Email = email;

        AddDomainEvent(new UsuarioAtualizadoEvent(
            UsuarioId: Id,
            Nome: Nome.Nome,
            Email: Email.Endereco));

        SetDataAtualizacao();
    }

    public void DesativarPerfil()
    {
        if (Status == StatusUsuario.Inativo)
            return;

        Status = StatusUsuario.Inativo;

        AddDomainEvent(new UsuarioDesativadoEvent(
            UsuarioId: Id));

        SetDataAtualizacao();
    }

    public void AtivarUsuario()
    {
        if (Status == StatusUsuario.Ativo)
            return;

        Status = StatusUsuario.Ativo;

        AddDomainEvent(new UsuarioAtivadoEvent(
            UsuarioId: Id));

        SetDataAtualizacao();
    }

    public void ReativarUsuario()
    {
        if (Status == StatusUsuario.Ativo)
            return;

        Status = StatusUsuario.Ativo;

        AddDomainEvent(new UsuarioReativadoEvent(
            UsuarioId: Id));

        SetDataAtualizacao();
    }

    private static void Validar(
        NomeUsuario nome,
        Email email)
    {
        Guard.AgainstNull(nome, nameof(nome));
        Guard.AgainstNull(email, nameof(email));
    }

}

