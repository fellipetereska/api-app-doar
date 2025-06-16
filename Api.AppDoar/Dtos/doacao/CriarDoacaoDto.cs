using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos.doacao
{
    public class CriarDoacaoDto
    {
        [Required]
        public int DoadorId { get; set; }

        [Required]
        public int InstituicaoId { get; set; }

        public string HorarioRetirada { get; set; } = string.Empty;

        public string Endereco { get; set; } = string.Empty;

        [Required]
        public string TipoEntrega { get; set; } 

        [Required]
        [MinLength(1, ErrorMessage = "Pelo menos um item é obrigatório")]
        public List<ItemDoacaoDto> Itens { get; set; } = new List<ItemDoacaoDto>();
    }

    public class ItemDoacaoDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = "usado";

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        public int SubcategoriaId { get; set; }

        public List<IFormFile>? ImagensItem { get; set; }
    }
}