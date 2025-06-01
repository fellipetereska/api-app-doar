using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos.doacao
{
    public class AtualizarStatusDto
    {
        [Required]
        public string Status { get; set; }
    }
}