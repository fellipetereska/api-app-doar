public class TermoDoacaoDto
{
    public int Id { get; set; }
    public string NomeInstituicao { get; set; } = string.Empty;
    public string CnpjInstituicao { get; set; } = string.Empty;

    public string NomeAssistido { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string DocumentoAssistido { get; set; } = string.Empty;
    public string EnderecoAssistido { get; set; } = string.Empty;

    public DateTime DataEntrega { get; set; }
    public string TipoEntrega { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;

    public List<ItemTermoDto> Itens { get; set; } = new();
}

public class ItemTermoDto
{
    public string Categoria { get; set; } = string.Empty;
    public string Subcategoria { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
