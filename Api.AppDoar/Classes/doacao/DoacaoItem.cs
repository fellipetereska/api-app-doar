using Dapper.Contrib.Extensions;

[Table("doacao_item")]
public class DoacaoItem
{
    [ExplicitKey]
    public int id { get; set; }

    public int doacao_id { get; set; }
    public string nome { get; set; }
    public string descricao { get; set; }
    public string estado { get; set; }
    public int quantidade { get; set; }
    public int subcategoria_id { get; set; }
}