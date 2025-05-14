using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos
{
    public class CriarDoacaoDto
    {
        [Required]
        public int doador_id { get; set; }

        public int? instituicao_id { get; set; }

        public string horario_retirada { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;

        [Required]
        public string tipo_entrega { get; set; } = string.Empty;

        [Required]
        public int usuario_id { get; set; }

        public List<IFormFile> ImagensDoacao { get; set; } = new List<IFormFile>(); 

        [Required]
        public List<ItemDoacaoDto> Itens { get; set; } = new List<ItemDoacaoDto>(); 
    }

    public class ItemDoacaoDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = string.Empty;

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public int SubcategoriaId { get; set; }

        public List<IFormFile> ImagensItem { get; set; } = new List<IFormFile>(); 
    }
}