namespace Coleta_TeorDeCinzas.Models
{
    public class InstrumentosModel
    {
        public int Id { get; set; }
        public string codigo { get; set; }
        public string? descricao { get; set; }
        public DateOnly validade { get; set; }
    }
}
