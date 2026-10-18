using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tarefas.Domain.Tests")]
[assembly: InternalsVisibleTo("Tarefas.Application")]

// O atributo[assembly: InternalsVisibleTo(...)] informa ao compilador que:
// Tudo que for internal neste assembly(Vendas.Domain)
// pode ser acessado pelo assembly Vendas.Domain.Tests.