namespace Api.AppDoar.Dtos
{
    public class DoacaoCategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<DoacaoSubcategoriaDto> Subcategorias { get; set; } = new();
    }

    public class DoacaoSubcategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}