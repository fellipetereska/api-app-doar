using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("entrega_itens")]
    public class ItensEntrega
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int quantidade { get; set; }

        [Required]
        public int entregas_id { get; set; }

        [Required]
        public int estoque_id { get; set; }
        
        [Required]
        public int categoria_id { get; set; }
        
        [Required]
        public int subcategoria_id { get; set; }

    }
}
