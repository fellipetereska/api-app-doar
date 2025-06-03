using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("projeto")]
    public class Projeto
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string nome { get; set; } = string.Empty;
        public string apelido { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        [Required]
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public bool? status { get; set; } = true;
        public int instituicao_id { get; set; }
    }
}
