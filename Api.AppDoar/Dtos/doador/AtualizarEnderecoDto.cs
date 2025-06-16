using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos.doador
{
    public class AtualizarEnderecoDto
    {
        [Required]
        public string Cep { get; set; }

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
        [StringLength(2, MinimumLength = 2)]
        public string Uf { get; set; }

    }
}
