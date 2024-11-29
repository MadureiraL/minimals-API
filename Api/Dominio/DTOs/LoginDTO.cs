using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimals_API.Dominio.DTOs
{
    // propriedades de login e senha
    public class LoginDTO
    {
        public string Email { get; set; } = default!;

        public string Senha { get; set; } = default!;
    }
}