using Api.AppDoar.Enum;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Dtos.doacao
{
    public class AtualizarStatusConfirmacaoDto
    {
        [Required]
        public StatusDoacao status { get; set; }
    }

    public class AtualizarStatusEntregaDto
    {
        [Required]
        public StatusEntrega status_entrega { get; set; }
    }

}