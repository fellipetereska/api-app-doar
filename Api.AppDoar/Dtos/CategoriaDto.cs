namespace Api.AppDoar.Dtos
{
    public class CategoriaDto
    {
        public string nome { get; set; } = string.Empty;
        public int instituicaoId { get; set; }
        public List<string>? subcategorias { get; set; }
    }
}
