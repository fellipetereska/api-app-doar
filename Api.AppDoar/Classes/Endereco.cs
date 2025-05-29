using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("endereco")]
    public class Endereco
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int usuario_id { get; set; }

        [Required]
        public string logradouro { get; set; }

        [Required]
        public string numero { get; set; }

        public string complemento { get; set; }

        [Required]
        public string bairro { get; set; }

        [Required]
        public string cidade { get; set; }

        [Required]
        public string uf { get; set; }

        [Required]
        public string cep { get; set; }

        public bool principal { get; set; } = false;
    }
}
