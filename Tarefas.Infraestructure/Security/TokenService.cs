
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Domain.Usuarios.Enums;

namespace Tarefas.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarAccessToken(
            Guid usuarioId,
            string nome,
            string email,
            PerfilUsuario perfil)
        {
            // Obtém as configurações do JWT.
            var secretKey = _configuration["JWT:SecretKey"]
                ?? throw new InvalidOperationException(
                    "A chave secreta JWT não foi configurada.");

            var issuer = _configuration["JWT:ValidIssuer"]
                ?? throw new InvalidOperationException(
                    "O Issuer JWT não foi configurado.");

            var audience = _configuration["JWT:ValidAudience"]
                ?? throw new InvalidOperationException(
                    "O Audience JWT não foi configurado.");

            var validade = _configuration.GetValue<int>(
                "JWT:TokenValidityInMinutes");

            if (Encoding.UTF8.GetByteCount(secretKey) < 32)
                throw new InvalidOperationException(
                    "A chave secreta JWT deve possuir pelo menos 32 bytes.");

            if (validade <= 0)
                throw new InvalidOperationException(
                    "A validade do JWT deve ser maior que zero.");

            // Converte a chave secreta em bytes.
            var privateKey = Encoding.UTF8.GetBytes(secretKey);

            // Define a assinatura digital.
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(privateKey),
                SecurityAlgorithms.HmacSha256);

            // Cria as Claims do usuário.
            var claims = new List<Claim>
{
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuarioId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    nome),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    perfil.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
};

            // Define as informações do token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddMinutes(validade),

                Issuer = issuer,

                Audience = audience,

                SigningCredentials = signingCredentials
            };

            // Gera o JWT.
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Converte o JWT para string.
            return tokenHandler.WriteToken(token);
        }
    }
}
