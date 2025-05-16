using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string nome { get; set; } = string.Empty;

        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string senha { get; set; } = string.Empty;

        [Required]
        public string role { get; set; } = string.Empty;

        public string telefone { get; set; } = string.Empty;

        public string cep { get; set; } = string.Empty;

        public string logradouro { get; set; } = string.Empty;

        public string endereco { get; set; } = string.Empty;

        public string numero { get; set; } = string.Empty;

        public string complemento { get; set; } = string.Empty;

        public string bairro { get; set; } = string.Empty;

        public string cidade { get; set; } = string.Empty;

        public string uf { get; set; } = string.Empty;

        [Required]
        public string tipo_documento { get; set; } = string.Empty;

        [Required]
        public string documento { get; set; } = string.Empty;

        public string tipo { get; set; } = "administrador";

        public int status { get; set; } = 1;

        public int? instituicao_id { get; set; }
    }
}
