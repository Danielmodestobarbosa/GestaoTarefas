using Tarefas.Domain.Integration.Usuarios;

namespace Tarefas.Infrastructure.Fakes;

public sealed class FakeUsuarioGateway : IUsuarioGateway
{
    private static readonly Dictionary<int, UsuarioExternoDto> _usuarios = new()
    {
        [1] = new(
            id: 1,
            nome: "Daniel Modesto",
            email: "daniel@email.com"
        ),
        [2] = new(
            id: 2,
            nome: "Maria Silva",
            email: "maria@email.com"
        )
    };

    public Task<UsuarioExternoDto?> ObterUsuarioPorIdAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (_usuarios.TryGetValue(usuarioId, out var usuario))
            return Task.FromResult<UsuarioExternoDto?>(usuario);

        return Task.FromResult<UsuarioExternoDto?>(null);
    }
}