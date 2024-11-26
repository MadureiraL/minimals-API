using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minimals_API.Dominio.Entidades;

namespace Minimals_API.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {

        public DbContexto(DbContextOptions<DbContexto> options) : base(options)
        {

        }
        public DbSet<Administrador> Administradores { get; set; } = default!;

        public DbSet<Veiculo> Veiculos { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Email = "administrador@teste.com",
                    Senha = "123456",
                    Perfil = "Adm"

                }


            );
        }
    }
}