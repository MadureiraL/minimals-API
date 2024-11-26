using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimals_API.Dominio.Enuns;

namespace Minimals_API.Dominio.DTOs
{
    public class AdminstradorDTO
    {
        public string Email { get; set; } = default!;

        public string Senha { get; set; } = default!;

        public Perfil? Perfil { get; set; } = default!;
    }
}