using Api.AppDoar.Classes.assistido;
using Api.AppDoar.Classes;

namespace Api.AppDoar.Dtos
{
    public class AssistidoComProjetosDto
    {
        public Assistido assistido { get; set; } = new();
        public List<Projeto> projetos { get; set; } = new();
    }

}
