using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos
{
    public class AtualizarStatusDto
    {
        [Required]
        public string Status { get; set; }
    }
}