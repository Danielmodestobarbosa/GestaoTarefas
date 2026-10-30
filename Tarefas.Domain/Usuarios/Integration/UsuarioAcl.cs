namespace Tarefas.Domain.Integration.Usuarios;

// A ACL traduz dados externos para conceitos compreendidos pelo domínio.
// Não conhece HTTP.
// Não conhece Infrastructure.
// Não conhece repositórios.
public sealed class UsuarioAcl
{
    public (string Nome, string Email) TraduzirUsuario(UsuarioExternoDto dto)
    {
        return (dto.Nome, dto.Email);
    }
}