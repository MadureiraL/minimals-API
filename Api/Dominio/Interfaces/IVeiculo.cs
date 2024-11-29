using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimals_API.Dominio.Entidades;

namespace Minimals_API.Dominio.Interfaces
{
    public interface IVeiculo
    {
        List<Veiculo> Todos(int? pagina = 1, string? nome = null, string marca = null);
        Veiculo? BuscaPorId(int id);

        void Incluir(Veiculo veiculo);

        void Atualizar(Veiculo veiculo);

        void Apagar(Veiculo veiculo);
    }
}