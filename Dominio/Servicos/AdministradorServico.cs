using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Minimals_API.Dominio.DTOs;
using Minimals_API.Dominio.Entidades;
using Minimals_API.Dominio.Interfaces;
using Minimals_API.Infraestrutura.Db;

namespace Minimals_API.Dominio.Servicos
{
    public class AdministradorServico : IAdministrador
    {
        private readonly DbContexto _contexto;


        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
        }

        public Administrador Incluir(Administrador adminstrador)
        {
            _contexto.Administradores.Add(adminstrador);
            _contexto.SaveChanges();

            return adminstrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();

            int intensPorPagina = 10;

            if (pagina != null)
            {
                query = query.Skip(((int)pagina - 1) * intensPorPagina).Take(intensPorPagina);
            }


            return query.ToList();
        }
    }
}