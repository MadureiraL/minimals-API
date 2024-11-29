using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimals_API.Dominio.DTOs;
using Minimals_API.Dominio.Entidades;

namespace Minimals_API.Dominio.Interfaces
{
    public interface IAdministrador
    {
        Administrador? Login(LoginDTO loginDTO);

        Administrador Incluir(Administrador adminstrador);

        Administrador? BuscaPorId(int id);
        List<Administrador> Todos(int? pagina);

    }
}