namespace Api.AppDoar.Dtos.instituicao
{
    public class CadastroEstoqueDto
    {
        public int instituicao_id { get; set; }
        public int categoria_id { get; set; }
        public int subcategoria_id { get; set; }
        public int quantidade { get; set; }
        public string descricao { get; set; } = string.Empty;
    }
}
