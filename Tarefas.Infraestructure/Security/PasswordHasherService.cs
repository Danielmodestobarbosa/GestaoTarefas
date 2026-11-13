
using Microsoft.AspNetCore.Identity;
using Tarefas.Application.Abstractions.Security;

namespace Tarefas.Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            new object(),
            password);
    }

    public bool VerifyPassword(
        string passwordHash,
        string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) ||
            string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            providedPassword);

        return result != PasswordVerificationResult.Failed;
    }
}

