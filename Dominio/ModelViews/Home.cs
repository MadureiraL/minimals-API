using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimals_API.Dominio.ModelViews
{
    public struct Home
    {   
        public string Mensagem { get => "Bem vindo a API de veiculos - Minimal API"; }
        public string Doc{ get => "/swagger"; }
    }
}