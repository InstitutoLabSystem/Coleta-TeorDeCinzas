using System.ComponentModel.DataAnnotations;

namespace Coleta_TeorDeCinzas.Models
{
    public class InicioColetaModel
    {
        [Key]
        public int Id { get; set; }
        public string os { get; set; }
        public string orcamento { get; set; }
        public DateOnly data_ini { get; set; }
        public DateOnly data_term { get; set; }
        public string? normativa { get; set; }
        public string? complementar { get; set; }
        public string? cime { get; set; }
        public string? inceterza { get; set; }
        public float minimo { get; set; }
        public float maximo { get; set; }
        public string? executador { get; set; }
    }
}
