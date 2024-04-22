using System.ComponentModel.DataAnnotations;

namespace Coleta_TeorDeCinzas.Models
{
    public class ColetaModel
    {
        [Key]
        public int Id { get; set; }
        public string os { get; set; }
        public string orcamento { get; set; }
        public string? parametro_enc { get; set; }
        public string? parametro_enc_dois { get; set; }
        public float amostra_um { get; set; }
        public float amostra_dois { get; set; }
        public float amostra_tres { get; set; }
        public float duplicata_um { get; set; }
        public float duplicata_dois { get; set; }
        public float duplicata_tres { get; set; }
        public float amostra_quatro { get; set; }
        public float duplicata_quatro { get; set; }
        public string? resultado { get; set; }
        public string? observacao { get; set; }
        public string? conformidade { get; set; }
        public string? decisao { get; set; }
        public string? executador { get; set; }

    }
}
