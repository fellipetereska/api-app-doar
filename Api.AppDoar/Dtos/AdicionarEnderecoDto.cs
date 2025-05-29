using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos
{
    public class AdicionarEnderecoDto
    {
        [Required]
        public string Logradouro { get; set; }

        [Required]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        [Required]
        public string Bairro { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required]
        public string Uf { get; set; }

        [Required]
        public string Cep { get; set; }

        public bool Principal { get; set; } = false;
    }
}
