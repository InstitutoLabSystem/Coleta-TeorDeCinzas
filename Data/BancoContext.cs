using Coleta_TeorDeCinzas.Models;
using Microsoft.EntityFrameworkCore;

namespace Coleta_TeorDeCinzas.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }
        public DbSet<AcessModel.Usuario> usuario { get; set; }
        public DbSet<OrdemServicoLaboratorio> ordemservico_laboratorio { get; set; }

    }
}
