using System.Net.Http.Json;
using Tarefas.Domain.Integration.Usuarios;

namespace Tarefas.Infrastructure.Services.Integration.Usuarios;

public sealed class UsuarioGateway : IUsuarioGateway
{
    private readonly HttpClient _httpClient;

    public UsuarioGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UsuarioExternoDto?> ObterUsuarioPorIdAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<UsuarioExternoDto>(
            $"users/{usuarioId}",
            cancellationToken);
    }
}