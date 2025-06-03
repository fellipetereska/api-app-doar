using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("assistido_projeto")]
    public class AssistidoProjeto
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int projeto_id { get; set; }
        [Required]
        public int assistido_id { get; set; }

    }
}
