using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Coleta_TeorDeCinzas.Models
{
    public class OrdemServicoLaboratorio
    {
        [Key]
        public string orcamento { get; set; }
        public string OS { get; set; }
        public string Laboratorio { get; set; }
        public string Status { get; set; }
        public string? Descricao { get; set; }
        public string Andamento { get; set; }
    }
}
