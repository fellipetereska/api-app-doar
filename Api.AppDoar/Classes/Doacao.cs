using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("doacao")]
    public class Doacao
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int doador_id { get; set; }

        public int? instituicao_id { get; set; }

        public string horario_retirada { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;

        [Required]
        public string tipo_entrega { get; set; } = string.Empty; 

        public string status { get; set; } = "pendente"; 

        [Required]
        public int usuario_id { get; set; }

        public bool status_entrega { get; set; } = false;

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}