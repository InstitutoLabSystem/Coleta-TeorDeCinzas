using Coleta_TeorDeCinzas.Models;
using Microsoft.EntityFrameworkCore;

namespace Coleta_TeorDeCinzas.Data
{
    public class QuimicoContext : DbContext
    {
        public QuimicoContext(DbContextOptions<QuimicoContext> options) : base(options)
        {
        }
        public DbSet<InicioColetaModel> registro_teor_cinzas { get; set; }
        public DbSet<InstrumentosModel> instrumentos_teor_cinzas { get; set; }
    }
}
