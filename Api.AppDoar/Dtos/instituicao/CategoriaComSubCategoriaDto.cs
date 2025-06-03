namespace Api.AppDoar.Dtos.instituicao
{
    public class CategoriaComSubCategoriaDto
    {
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public int? SubcategoriaId { get; set; }
        public string? SubcategoriaNome { get; set; }
    }
}
