namespace Coleta_TeorDeCinzas.Models
{
    public class InformacaoInstrumentos
    {
        public int Id { get; set; }
        public string os { get; set; }
        public string orcamento { get; set; }
        public string? codigo { get; set; }
        public string? descricao { get; set; }
        public DateOnly? validade { get; set; }
    }
}
