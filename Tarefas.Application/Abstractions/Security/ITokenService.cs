
using Tarefas.Domain.Usuarios.Enums;

public interface ITokenService
{
    string GerarAccessToken(
        Guid usuarioId,
        string nome,
        string email,
        PerfilUsuario perfil);
}