using System.ComponentModel.DataAnnotations;

namespace Coleta_TeorDeCinzas.Models
{
    public class AcessModel
    {
        public class Usuario
        {
            [Key]
            public int pk { get; set; }
            public string Nome_Usuario { get; set; }
            public string Senha_Usuario { get; set; }
            public string nomecompleto { get; set; }
            public string cargo { get; set; }
            public string setor { get; set; }
            public string laboratorio { get; set; }
            public int ativo { get; set; }
        }
    }
}
