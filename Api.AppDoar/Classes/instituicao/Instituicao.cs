using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes.instituicao
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
        public string cep { get; set; } = string.Empty;

        [Required]
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

        public string descricao { get; set; } = string.Empty;

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public string? logo_path { get; set; } 

    }
}
