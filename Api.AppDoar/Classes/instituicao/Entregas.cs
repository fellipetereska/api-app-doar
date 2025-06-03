using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes.instituicao
{
    [Table("entregas")]
    public class Entregas
    {
        [Key]
        public int id { get; set; }

        [Required]
        public DateTime? data { get; set; }
        
        public string observacao { get; set; } = string.Empty;

        [Required]
        public int? instituicao_id { get; set; }

        [Required]
        public int? assistido_id { get; set; }

        [Required]
        public string tipo_entrega { get; set; } = "entregar";

        [Required]
        public string status { get; set; } = "pendente";
    }
}
