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
        public string tipo_documento { get; set; } = string.Empty;
        
        [Required]
        public string documento { get; set; } = string.Empty;

        public string telefone { get; set; } = string.Empty;

        [Required]
        public string cep {  get; set; } = string.Empty;

        public string logradouro { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;

        [Required]
        public string numero { get; set; } = string.Empty;

        public string complemento { get; set; } = string.Empty;

        [Required]
        public string bairro { get; set; } = string.Empty;

        [Required]
        public string cidade { get; set; } = string.Empty;

        [Required]
        public string uf { get; set; } = string.Empty;

        public string latitude { get; set; } = string.Empty;
        public string longitude { get; set; } = string.Empty;

        public int status_lista_espera { get; set; } = 0;

        [Required]
        public int instituicao_id { get; set; }

    }
}
