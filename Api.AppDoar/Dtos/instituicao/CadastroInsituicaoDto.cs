using Api.AppDoar.Classes.instituicao;

namespace Api.AppDoar.Dtos.instituicao
{
    public class CadastroInstituicaoDto
    {
        public CadastroUsuarioDto usuario { get; set; }
        public Instituicao instituicao { get; set; }
    }

}
