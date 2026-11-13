using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Abstractions.Security;

public interface IPasswordHasher
{
    //Transforma a senha em hash seguro
    string HashPassword(string password);

    //Verifica se a senha fornecida corresponde ao hash armazenado
    bool VerifyPassword(
        string hashedPassword,
        string providedPassword);
}

