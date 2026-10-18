using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Base;

namespace Tarefas.Domain.Usuarios.Events;

public sealed record UsuarioAtivadoEvent(
    Guid UsuarioId) : DomainEventBase;
