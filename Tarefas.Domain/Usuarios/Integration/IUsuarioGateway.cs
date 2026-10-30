namespace Tarefas.Domain.Integration.Usuarios;

// Define o contrato para consultar usuários em um sistema externo.
public interface IUsuarioGateway
{
    Task<UsuarioExternoDto?> ObterUsuarioPorIdAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
}