using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("instituicao")]
    public class Instituicao
    {
        [Key]
        public int id { get; set; }
        
        [Required]
        public string nome { get; set; } = string.Empty;

        [Required]
        public string cnpj { get; set; } = string.Empty;

        public string telefone { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;
        
        public string descricao { get; set; } = string.Empty;

        public Decimal latitude { get; set; }

        public Decimal longitude { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
