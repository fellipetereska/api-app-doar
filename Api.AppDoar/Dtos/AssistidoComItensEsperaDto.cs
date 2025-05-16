using Api.AppDoar.Classes;

namespace Api.AppDoar.Dtos
{
    public class AssistidoComItensEsperaDto
    {
        public Assistido Assistido { get; set; } = new();
        public List<ListaEspera> ItensEspera { get; set; } = new();
    }

}
