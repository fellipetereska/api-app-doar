using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("assistido")]
    public class Assistido
    {

        [Key]
        public int id { get; set; }

        [Required]
        public string nome { get; set; } = string.Empty;

        [Required]
        public string documento { get; set; } = string.Empty;

        public string telefone { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;

        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }

        public int status_lista_espera { get; set; } = 0;

    }
}
