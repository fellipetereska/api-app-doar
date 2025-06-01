using Api.AppDoar.Classes.assistido;
using Api.AppDoar.Classes.instituicao;

namespace Api.AppDoar.Dtos.assistido;

public class AssistidoComItensEsperaDto
{
    public Assistido Assistido { get; set; } = new();
    public List<ListaEspera> ItensEspera { get; set; } = new();
}
